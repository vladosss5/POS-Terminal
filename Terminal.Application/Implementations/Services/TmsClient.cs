using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

public class TmsClient : ITmsClient
{
    private readonly TmsClientConfig _config = new();
    private readonly ILogger<TmsClient> _logger;
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private SerialPort? _serialPort;
    private Stream? _activeStream;
    private readonly object _lock = new();
    private bool _isConnected;
    private bool _isCrypto;
    private ushort _crc16;
    private byte _state = StateWaitFend;
    private bool _isFend;
    private byte _protocolState;
    private ushort _packetLen;
    private ushort _packetOffset;
    private byte _packetCmd;
    private byte[]? _packetData;
    private int _packetDataIndex;
    private ushort _receivedCrc;
    
    // Состояния протокола
    private const byte StateWaitFend = 0;
    private const byte StateWaitCmd = 1;
    private const byte StateWaitMaxFrame = 2;
    private const byte StateWaitCurFrame = 3;
    private const byte StateWaitLenL = 4;
    private const byte StateWaitLenH = 5;
    private const byte StateWaitData = 6;
    private const byte StateWaitCrcL = 7;
    private const byte StateWaitCrcH = 8;
    
    // Управляющие символы
    private const byte Fend = 0xC0;
    private const byte Fesk = 0xDB;
    private const byte Tadd = 0x1C;
    private const byte Tfend = Fend + Tadd;
    private const byte Tfesk = Fesk + Tadd;
    
    // Константы протокола
    private const int TerminalNumberLen = 12;
    private const int VersionLen = 20;
    private const int ProtocolVersionLen = 1;
    private const int ProtocolMaskLen = 2;

    private readonly IParameterService _parameterService;
    private readonly SemaphoreSlim _packetSemaphore = new(0, 1);
    private IPacket? _receivedPacket;
    
    private TaskCompletionSource<IPacket?>? _pendingResponse;
    private readonly object _pendingLock = new();
    public bool IsConnected => _isConnected;
    
    public TmsClient(
        ILogger<TmsClient> logger, 
        IParameterService parameterService)
    {
        _logger = logger;
        _parameterService = parameterService;
        
        ConfigureThis().GetAwaiter().GetResult();
    }

    private async Task ConfigureThis()
    {
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        _config.TerminalNumber = terminalNumber;
    }
    
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        await DisconnectAsync();
        
        try
        {
            switch (_config.ConnectionType)
            {
                case ConnectType.TcpIp:
                    _tcpClient = new TcpClient();
                    await _tcpClient.ConnectAsync(_config.Host, _config.Port, cancellationToken);
                    _stream = _tcpClient.GetStream();
                    _activeStream = _stream;
                    break;
                    
                case ConnectType.SerialPort:
                    _serialPort = new SerialPort(_config.ComPort, _config.BaudRate, Parity.None, 8, StopBits.One);
                    _serialPort.Open();
                    _activeStream = _serialPort.BaseStream;
                    break;
                    
                default:
                    throw new NotSupportedException($"Connection type {_config.ConnectionType} not supported");
            }
            
            _isConnected = true;
            _logger.LogInformation($"Connected to TMS server via {_config.ConnectionType}");
            
            _ = Task.Run(() => ReceivePacketsAsync(cancellationToken), cancellationToken);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection failed: {ex.Message}");
            _isConnected = false;
            return false;
        }
    }
    
    public async Task DisconnectAsync()
    {
        _isConnected = false;
        
        try
        {
            await EndDialogAsync(CancellationToken.None);
        }
        catch { }
        
        _activeStream?.Close();
        _activeStream?.Dispose();
        _tcpClient?.Close();
        _tcpClient?.Dispose();
        _serialPort?.Close();
        _serialPort?.Dispose();
        
        _activeStream = null;
        _tcpClient = null;
        _serialPort = null;
        
        _logger.LogInformation("Disconnected from TMS server");
    }
    
    public async Task<AuthorizationResult> AuthorizeAsync(CancellationToken cancellationToken = default)
    {
        var result = new AuthorizationResult();
        
        try
        {
            // 1. Отправляем StartPacket
            _logger.LogDebug("Sending StartPacket");
            var startPacket = new SncPacket((byte)SncProtocolCode.StartPacket, 0, Array.Empty<byte>());
            await WritePacketAsync(startPacket, cancellationToken);
            
            // Небольшая задержка перед отправкой Authorize
            await Task.Delay(100, cancellationToken);
            
            // 2. Формируем пакет авторизации
            var packetLen = TerminalNumberLen + VersionLen + ProtocolVersionLen;
            var data = new byte[packetLen];
            
            // Номер терминала
            var terminalBytes = Encoding.ASCII.GetBytes(_config.TerminalNumber.PadRight(TerminalNumberLen, '\0'));
            Array.Copy(terminalBytes, 0, data, 0, Math.Min(terminalBytes.Length, TerminalNumberLen));
            
            // Версия ПО
            var versionBytes = Encoding.ASCII.GetBytes(_config.Version.PadRight(VersionLen, '\0'));
            Array.Copy(versionBytes, 0, data, TerminalNumberLen, Math.Min(versionBytes.Length, VersionLen));
            
            // Версия протокола
            data[TerminalNumberLen + VersionLen] = 0x01;
            
            var packet = new SncPacket((byte)SncProtocolCode.Authorize, 0, data);
            
            _logger.LogInformation("Sending Authorize packet, waiting for response...");
            
            // 3. Отправляем и ждем ответ - ИСПОЛЬЗУЕМ возвращаемый Response
            var (success, responsePacket) = await WritePacketWithWaitAsync(packet, timeoutMs: _config.TimeoutMs, cancellationToken);
            
            if (!success || responsePacket == null)
            {
                result.ErrorMessage = "No response for authorize";
                _logger.LogError(result.ErrorMessage);
                return result;
            }
            
            _logger.LogInformation("Received authorize response, Cmd:0x{Command:X2}", responsePacket.Cmd);
            
            var response = responsePacket as SncPacket;
            if (response?.Data == null || response.Data.Length < 1)
            {
                result.ErrorMessage = "Invalid authorize response";
                return result;
            }
            
            result.ErrorCode = response.Data[0];
            if (result.ErrorCode != 0)
            {
                result.ErrorMessage = $"Authorization failed with code {result.ErrorCode}";
                return result;
            }
            
            if (response.Data.Length >= 2)
            {
                result.Flags = (AuthorizeFlags)response.Data[1];
            }
            
            var offset = 2;
            
            // Синхронизация времени
            if (result.Flags.HasFlag(AuthorizeFlags.SynchroTime) && response.Data.Length >= offset + 8)
            {
                var dateValue = BitConverter.ToUInt32(response.Data, offset);
                var timeValue = BitConverter.ToUInt32(response.Data, offset + 4);
                
                result.ServerTime = new DateTime(
                    (int)(dateValue / 10000),
                    (int)((dateValue / 100) % 100),
                    (int)(dateValue % 100),
                    (int)(timeValue / 10000),
                    (int)((timeValue / 100) % 100),
                    (int)(timeValue % 100)
                );
                offset += 8;
            }
            
            // Шифрование
            if (result.Flags.HasFlag(AuthorizeFlags.Crypto))
            {
                _isCrypto = true;
            }
            
            result.Success = true;
            _logger.LogInformation($"Authorization successful, flags: {result.Flags}");
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            _logger.LogError($"Authorization failed: {ex.Message}");
        }
        
        return result;
    }
    
    public async Task<ReceiveResult> ReceiveTablesAsync(CancellationToken cancellationToken = default)
    {
        var result = new ReceiveResult();
        
        try
        {
            var packet = new SncPacket((byte)SncProtocolCode.Receive, 0, []);
            
            if (!(await WritePacketWithWaitAsync(packet, timeoutMs: 20000, cancellationToken)).Success)
            {
                result.Success = false;
                return result;
            }
            
            var response = _receivedPacket as SncPacket;
            if (response?.Data == null || response.Data.Length < 3 || response.Data[0] != 0)
            {
                result.Success = false;
                return result;
            }
            
            result.PacketCount = response.Data[1] | (response.Data[2] << 8);
            
            for (ushort i = 0; i < result.PacketCount && !cancellationToken.IsCancellationRequested; i++)
            {
                await OnUpdateProgressAsync($"Получение пакета {i + 1} из {result.PacketCount}");
                
                var dataPacket = new SncPacket((byte)SncProtocolCode.DataI, i, BitConverter.GetBytes(i));
                
                if (!(await WritePacketWithWaitAsync(dataPacket, timeoutMs: _config.TimeoutMs, cancellationToken)).Success)
                {
                    _logger.LogWarning($"Failed to receive packet {i}");
                    continue;
                }
                
                var dataResponse = _receivedPacket as SncPacket;
                
                if (dataResponse?.Data is not { Length: > 0 }) 
                    continue;
                
                var fileName = Path.Combine(GetIncomingPath(), $"file{i:D3}.zip");
                await File.WriteAllBytesAsync(fileName, dataResponse.Data, cancellationToken);
                result.SavedFiles.Add(fileName);
            }
            
            var endPacket = new SncPacket((byte)SncProtocolCode.EndFile, 0, []);
            await WritePacketWithWaitAsync(endPacket, timeoutMs: _config.TimeoutMs, cancellationToken);
            
            result.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Receive tables failed: {ex.Message}");
            result.Success = false;
        }
        
        return result;
    }
    
    public async Task<ReceiveResult> ReceiveUpdatesAsync(CancellationToken cancellationToken = default)
    {
        var result = new ReceiveResult();
        
        try
        {
            await OnUpdateProgressAsync("Запрос обновлений...");
            
            var packet = new SncPacket((byte)SncProtocolCode.ReceiveUpdate, 0, Array.Empty<byte>());
            
            if (!(await WritePacketWithWaitAsync(packet, timeoutMs: 20000, cancellationToken)).Success)
            {
                result.Success = false;
                return result;
            }
            
            var response = _receivedPacket as SncPacket;
            if (response?.Data == null || response.Data.Length < 3 || response.Data[0] != 0)
            {
                result.Success = false;
                return result;
            }
            
            result.PacketCount = response.Data[1] | (response.Data[2] << 8);
            
            var updatePath = Path.Combine(GetIncomingPath(), "update.zip");
            
            for (ushort i = 0; i < result.PacketCount && !cancellationToken.IsCancellationRequested; i++)
            {
                await OnUpdateProgressAsync($"Загрузка обновления {i + 1} из {result.PacketCount}");
                
                var dataPacket = new SncPacket((byte)SncProtocolCode.DataI, i, BitConverter.GetBytes(i));
                
                if (!(await WritePacketWithWaitAsync(dataPacket, timeoutMs: _config.TimeoutMs, cancellationToken)).Success)
                {
                    _logger.LogWarning($"Failed to receive update packet {i}");
                    continue;
                }
                
                var dataResponse = _receivedPacket as SncPacket;
                
                if (dataResponse?.Data is not { Length: > 0 }) 
                    continue;
                
                await using var fs = new FileStream(updatePath, FileMode.Append, FileAccess.Write);
                await fs.WriteAsync(dataResponse.Data, cancellationToken);
            }
            
            var endPacket = new SncPacket((byte)SncProtocolCode.EndUpdate, 0, Array.Empty<byte>());
            await WritePacketWithWaitAsync(endPacket, timeoutMs: _config.TimeoutMs, cancellationToken);
            
            result.Success = true;
            result.SavedFiles.Add(updatePath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Receive updates failed: {ex.Message}");
            result.Success = false;
        }
        
        return result;
    }
    
    public async Task<SendTableResult> SendTableAsync(string tableName, string keyField, byte[] data, CancellationToken cancellationToken = default)
    {
        var result = new SendTableResult();
        
        try
        {
            await OnUpdateProgressAsync($"Отправка {tableName}...");
            
            var packet = new SncPacket((byte)SncProtocolCode.SendTable, 0, data);

            var writingPocket = await WritePacketWithWaitAsync(packet, timeoutMs: _config.TimeoutMs, cancellationToken);
            
            if (!writingPocket.Success)
            {
                result.Success = false;
                return result;
            }
            
            var responsePacket = writingPocket.Response as SncPacket;
            if (responsePacket?.Data == null || responsePacket.Data.Length < 1 || responsePacket.Data[0] != 0)
            {
                result.Success = false;
                return result;
            }

            // Распарсить ответ с ключами (success, errors, errorsSave)
            // TODO: Расшифровка zip с результатами
            // Формат ответа: zip с файлами success, errors, errorsSave
            
            result.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Send table failed: {ex.Message}");
            result.Success = false;
        }
        
        return result;
    }
    
    public async Task<bool> SendFileAsync(string filePath, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                _logger.LogWarning($"File not found: {filePath}");
                return false;
            }
            
            var header = new FileHeader
            {
                BlockSize = (uint)(_config.PacketSizeKb * 1024),
                FileSize = (uint)fileInfo.Length,
                FileName = fileName
            };
            
            var headerBytes = header.ToBytes();
            var headerPacket = new SncPacket((byte)SncProtocolCode.SendFileHeader, 0, headerBytes);
            
            if (!(await WritePacketWithWaitAsync(headerPacket, timeoutMs: _config.TimeoutMs, cancellationToken)).Success)
            {
                return false;
            }
            
            var fileData = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var blockSize = (int)header.BlockSize;
            var offset = 0;
            
            for (ushort i = 0; offset < fileData.Length; i++)
            {
                var chunkSize = Math.Min(blockSize, fileData.Length - offset);
                var chunk = new byte[chunkSize];
                Array.Copy(fileData, offset, chunk, 0, chunkSize);
                
                var dataPacket = new SncPacket((byte)SncProtocolCode.SendFile, i, chunk);

                if (!(await WritePacketWithWaitAsync(dataPacket, timeoutMs: _config.TimeoutMs, cancellationToken)).Success)
                    return false;
                
                offset += chunkSize;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Send file failed: {ex.Message}");
            return false;
        }
    }
    
    public async Task EndDialogAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var packet = new SncPacket((byte)SncProtocolCode.EndDialog, 0, Array.Empty<byte>());
            await WritePacketAsync(packet, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"End dialog failed: {ex.Message}");
        }
    }
    
    #region Private Methods
    
    private async Task ReceivePacketsAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        
        while (_isConnected && _activeStream != null && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!_activeStream.CanRead) break;
                
                var bytesRead = await _activeStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead == 0) break;
                
                for (var i = 0; i < bytesRead; i++)
                {
                    ProcessByte(buffer[i]);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Receive error: {ex.Message}");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
    
    private void ProcessByte(byte data)
    {
        if (data == Fesk)
        {
            _isFend = true;
            return;
        }
        
        if (_isFend)
        {
            data -= Tadd;
            _isFend = false;
        }
        
        switch (_state)
        {
            case StateWaitFend:
                if (data == Fend)
                    _state = StateWaitCmd;
                break;
                
            case StateWaitCmd:
                _packetCmd = data;
                _state = StateWaitMaxFrame;
                break;
                
            case StateWaitMaxFrame:
                _packetOffset = data;
                _state = StateWaitCurFrame;
                break;
                
            case StateWaitCurFrame:
                _packetOffset += (ushort)(data << 8);
                _state = StateWaitLenL;
                break;
                
            case StateWaitLenL:
                _packetLen = data;
                _state = StateWaitLenH;
                break;
                
            case StateWaitLenH:
                _packetLen += (ushort)(data << 8);
                _packetData = new byte[_packetLen];
                _packetDataIndex = 0;
                _state = _packetLen == 0 ? StateWaitCrcL : StateWaitData;
                break;
                
            case StateWaitData:
                _packetData![_packetDataIndex++] = data;
                if (_packetDataIndex >= _packetLen)
                    _state = StateWaitCrcL;
                break;
                
            case StateWaitCrcL:
                _receivedCrc = data;
                _state = StateWaitCrcH;
                break;
                
            case StateWaitCrcH:
                _receivedCrc += (ushort)(data << 8);
                
                var packet = new SncPacket(_packetCmd, _packetOffset, _packetData);
                var calcCrc = packet.CalculateCrc16();
                
                if (calcCrc == _receivedCrc)
                {
                    _receivedPacket = packet;
                    
                    lock (_pendingLock)
                    {
                        _pendingResponse?.TrySetResult(packet);
                        _pendingResponse = null;
                    }
                    
                    _logger.LogDebug($"Packet received: Cmd={_packetCmd}, Len={_packetLen}");
                }
                else
                {
                    _logger.LogWarning($"CRC mismatch: expected={calcCrc}, received={_receivedCrc}");
                }
                
                ResetState();
                break;
        }
    }
    
    private void ResetState()
    {
        _state = StateWaitFend;
        _isFend = false;
        _packetCmd = 0;
        _packetOffset = 0;
        _packetLen = 0;
        _packetData = null;
        _packetDataIndex = 0;
        _receivedCrc = 0;
    }
    
    private async Task WritePacketAsync(IPacket packet, CancellationToken cancellationToken)
    {
        if (_activeStream == null) throw new InvalidOperationException("Not connected");
        
        var buffer = new List<byte> { Fend };
        
        WriteByte(buffer, packet.Cmd);
        WriteByte(buffer, (byte)(packet.Offset & 0xFF));
        WriteByte(buffer, (byte)(packet.Offset >> 8));
        WriteByte(buffer, (byte)(packet.Len & 0xFF));
        WriteByte(buffer, (byte)(packet.Len >> 8));
        
        for (var i = 0; i < packet.Len; i++)
            WriteByte(buffer, packet.Data[i]);
        
        var crc = packet.CalculateCrc16();
        WriteByte(buffer, (byte)(crc & 0xFF));
        WriteByte(buffer, (byte)(crc >> 8));
        
        await _activeStream.WriteAsync(buffer.ToArray(), cancellationToken);
        
        _logger.LogDebug($"Packet sent: Cmd={packet.Cmd}, Len={packet.Len}");
    }
    
    private void WriteByte(List<byte> buffer, byte data)
    {
        if (data == Fend)
        {
            buffer.Add(Fesk);
            buffer.Add(Tfend);
        }
        else if (data == Fesk)
        {
            buffer.Add(Fesk);
            buffer.Add(Tfesk);
        }
        else
        {
            buffer.Add(data);
        }
    }
    
    /// <summary>
    /// Отправить пакет и дождаться ответа
    /// </summary>
    /// <returns>Кортеж: (успех, ответный пакет)</returns>
    private async Task<(bool Success, IPacket? Response)> WritePacketWithWaitAsync(
        IPacket packet, 
        int timeoutMs = 5000, 
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<IPacket?>();
        
        lock (_pendingLock)
            _pendingResponse = tcs;
        
        try
        {
            await WritePacketAsync(packet, cancellationToken);
            _logger.LogDebug($"Packet sent: Cmd={packet.Cmd}, waiting for response...");
        
            using var cts = new CancellationTokenSource(timeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

            await using (linkedCts.Token.Register(() => tcs.TrySetResult(null)))
            {
                var response = await tcs.Task;

                if (response != null) 
                    return (true, response);
                
                _logger.LogWarning($"Timeout waiting for response to packet Cmd={packet.Cmd}");
                return (false, null);

            }
        }
        finally
        {
            lock (_pendingLock)
            {
                if (_pendingResponse == tcs)
                    _pendingResponse = null;
            }
        }
    }
    
    private string GetIncomingPath()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "tms", "incoming");
        Directory.CreateDirectory(path);
        return path;
    }
    
    // Событие для прогресса
    public event Func<string, Task>? UpdateProgress;
    
    private async Task OnUpdateProgressAsync(string message)
    {
        if (UpdateProgress != null)
            await UpdateProgress.Invoke(message);
    }
    
    #endregion
}