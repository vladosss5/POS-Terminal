using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

public class TmsConnectionService : ITmsConnectionService
{
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private readonly ILogger<TmsConnectionService> _logger;
    private bool _isCrypto;
    private bool _isSynchro;
    private PosFlags _posFlags;
    
    public event Action<ulong>? OnDataReceived;
    public bool IsConnected => _tcpClient?.Connected == true;

    public TmsConnectionService(ILogger<TmsConnectionService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ConnectAndAuthorizeAsync(ulong terminalId, string serverHost, int port, CancellationToken cancellationToken = default)
    {
        try
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(serverHost, port);
            _stream = _tcpClient.GetStream();
            _logger.LogInformation("[TMS] Подключен к {Host}:{Port}", serverHost, port);
 
            var startPacket = await ReceivePacketAsync(cancellationToken);
            if (startPacket?.Cmd != SncProtocolCode.StartPacket)
            {
                _logger.LogWarning("[TMS] Сервер не прислал StartPacket, но продолжаем");
            }
        
            // 3. Отправляем Authorize (0x05) с данными терминала
            var authData = BuildAuthorizePacket(terminalId);
            await SendPacketAsync(new Packet 
            { 
                Cmd = SncProtocolCode.Authorize, 
                Data = authData, 
                Length = (ushort)authData.Length 
            }, cancellationToken);

            // 4. Получение ответа
            var response = await ReceivePacketAsync(cancellationToken);
        
            if (response?.Cmd == SncProtocolCode.Authorize && response.Data.Length >= 2 && response.Data[0] == 0)
            {
                // ... авторизация успешна
                return true;
            }
        
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TMS] Ошибка подключения/авторизации");
            return false;
        }
    }

    public async Task SendDataAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        // [ЧАСТИЧНАЯ РЕАЛИЗАЦИЯ] — требуется уточнение формата
        // TMS ожидает данные через команду SendTable (0x12)
        var packet = new Packet
        {
            Cmd = SncProtocolCode.SendTable,
            Data = data,
            Length = (ushort)data.Length
        };
        await SendPacketAsync(packet, cancellationToken);
        
        // Ждем подтверждения
        var response = await ReceivePacketAsync(cancellationToken);
        if (response?.Cmd == SncProtocolCode.SendTable && response.Data.Length > 0 && response.Data[0] != 0)
        {
            _logger.LogError("[TMS] Ошибка отправки данных. Код: {Code}", response.Data[0]);
            throw new Exception($"SendData failed with code {response.Data[0]}");
        }
    }

    public async Task DisconnectAsync()
    {
        if (IsConnected && _stream != null)
        {
            try
            {
                await SendPacketAsync(new Packet { Cmd = SncProtocolCode.EndDialog }, CancellationToken.None);
            }
            catch { }
        }
        _stream?.Close();
        _tcpClient?.Close();
        _logger.LogInformation("[TMS] Соединение закрыто");
    }

    #region Private Methods (Protocol Implementation)
    
    private byte[] BuildAuthorizePacket(ulong terminalId)
    {
        // Формат из TMS: BaseChanel.Authorize ожидает:
        // [TerminalId: 12 байт] + [Version: 20 байт] + [ProtocolVersion: 1 байт] + [PosFlags: 2 байта]
        var result = new byte[12 + 20 + 1 + 2];
        
        // Terminal ID (12 байт, ASCII, нуль-терминированная строка)
        var idStr = terminalId.ToString();
        var idBytes = System.Text.Encoding.ASCII.GetBytes(idStr);
        Array.Copy(idBytes, result, Math.Min(idBytes.Length, 12));
        
        // Version (20 байт) — версия терминала
        var versionStr = "1.8.1"; // или из настроек терминала
        var verBytes = System.Text.Encoding.ASCII.GetBytes(versionStr);
        Array.Copy(verBytes, 0, result, 12, Math.Min(verBytes.Length, 20));
        
        // Protocol Version (1 байт) — 1 = расширенный протокол
        result[32] = 0x01;
        
        // PosFlags (2 байта) — поддерживаемые флаги (CRC32, GPRS и т.д.)
        var flagsBytes = BitConverter.GetBytes((ushort)PosFlags.CRC32);
        Array.Copy(flagsBytes, 0, result, 33, 2);
        
        return result;
    }
    
    private async Task SendPacketAsync(Packet packet, CancellationToken ct)
    {
        if (_stream == null) throw new InvalidOperationException("Not connected");
        
        using var ms = new MemoryStream();
        
        // Преамбула FEND (0xC0)
        ms.WriteByte(0xC0);
        
        // Cmd
        WriteEscapedByte(ms, (byte)packet.Cmd);
        
        // Offset (2 байта, little-endian)
        WriteEscapedByte(ms, (byte)(packet.Offset & 0xFF));
        WriteEscapedByte(ms, (byte)(packet.Offset >> 8));
        
        // Length (2 байта, little-endian)
        WriteEscapedByte(ms, (byte)(packet.Length & 0xFF));
        WriteEscapedByte(ms, (byte)(packet.Length >> 8));
        
        // Data
        for (int i = 0; i < packet.Length; i++)
        {
            WriteEscapedByte(ms, packet.Data[i]);
        }
        
        // CRC16 (2 байта)
        var crc = CalculateCrc16(ms.ToArray(), 1, ms.ToArray().Length - 1);
        WriteEscapedByte(ms, (byte)(crc & 0xFF));
        WriteEscapedByte(ms, (byte)(crc >> 8));
        
        var buffer = ms.ToArray();
        await _stream.WriteAsync(buffer, 0, buffer.Length, ct);
        
        _logger.LogDebug("[TMS] Отправлен пакет {Cmd}, размер {Size}", packet.Cmd, buffer.Length);
    }
    
    private async Task<Packet?> ReceivePacketAsync(CancellationToken ct)
    {
        if (_stream == null) throw new InvalidOperationException("Not connected");
        
        // Поиск FEND (0xC0)
        int b;
        while ((b = await ReadByteWithTimeoutAsync(ct)) != 0xC0)
        {
            if (b == -1) return null;
        }
        
        // Чтение Cmd
        var cmdByte = await ReadEscapedByteAsync(ct);
        if (cmdByte == null) return null;
        
        var packet = new Packet
        {
            Cmd = (SncProtocolCode)cmdByte.Value,
            Offset = 0,
            Length = 0
        };
        
        // Чтение Offset (2 байта)
        var offLow = await ReadEscapedByteAsync(ct);
        var offHigh = await ReadEscapedByteAsync(ct);
        if (offLow == null || offHigh == null) return null;
        packet.Offset = (ushort)(offLow.Value | (offHigh.Value << 8));
        
        // Чтение Length (2 байта)
        var lenLow = await ReadEscapedByteAsync(ct);
        var lenHigh = await ReadEscapedByteAsync(ct);
        if (lenLow == null || lenHigh == null) return null;
        packet.Length = (ushort)(lenLow.Value | (lenHigh.Value << 8));
        
        // Чтение Data
        packet.Data = new byte[packet.Length];
        for (int i = 0; i < packet.Length; i++)
        {
            var dataByte = await ReadEscapedByteAsync(ct);
            if (dataByte == null) return null;
            packet.Data[i] = dataByte.Value;
        }
        
        // Чтение CRC (2 байта)
        var crcLow = await ReadEscapedByteAsync(ct);
        var crcHigh = await ReadEscapedByteAsync(ct);
        if (crcLow == null || crcHigh == null) return null;
        
        _logger.LogDebug("[TMS] Получен пакет {Cmd}, размер {Size}", packet.Cmd, packet.Length);
        return packet;
    }
    
    private void WriteEscapedByte(MemoryStream ms, byte value)
    {
        if (value == 0xC0) // FEND
        {
            ms.WriteByte(0xC0);
            ms.WriteByte(0xE0); // TFEND = FEND + 0x20
        }
        else if (value == 0xDB) // FESC
        {
            ms.WriteByte(0xDB);
            ms.WriteByte(0xFB); // TFESC = FESC + 0x20
        }
        else
        {
            ms.WriteByte(value);
        }
    }
    
    private async Task<byte?> ReadEscapedByteAsync(CancellationToken ct)
    {
        var b = await ReadByteWithTimeoutAsync(ct);
        if (b == -1) return null;
        
        var value = (byte)b;
        if (value == 0xC0) // FEND
        {
            var next = await ReadByteWithTimeoutAsync(ct);
            if (next == -1) return null;
            if (next == 0xE0) return 0xC0;
            if (next == 0xDB) return 0xDB;
        }
        else if (value == 0xDB) // FESC
        {
            var next = await ReadByteWithTimeoutAsync(ct);
            if (next == -1) return null;
            if (next == 0xFB) return 0xDB;
        }
        
        return value;
    }
    
    private async Task<int> ReadByteWithTimeoutAsync(CancellationToken ct, int timeoutMs = 5000)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeoutMs);
    
        var buffer = new byte[1];
        try
        {
            var bytesRead = await _stream!.ReadAsync(buffer, 0, 1, cts.Token);
            return bytesRead == 1 ? buffer[0] : -1;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Чтение данных прервано по таймауту");
        }
    }
    
    private ushort CalculateCrc16(byte[] data, int startIndex, int length)
    {
        byte low = 0x28, high = 0x11;
        
        for (int i = startIndex; i < startIndex + length; i++)
        {
            byte b = data[i];
            for (int bit = 0; bit < 8; bit++)
            {
                var flag = (b & (1 << bit)) != 0;
                if ((low & 0x01) != 0) flag = !flag;
                
                if (flag)
                {
                    high ^= 0x40;
                    low ^= 0x02;
                }
                
                var flagC = (high & 0x01) != 0;
                high >>= 1;
                if (flag) high |= 0x80;
                
                low >>= 1;
                if (flagC) low |= 0x80;
            }
        }
        
        return (ushort)((high << 8) | low);
    }
    
    #endregion
}