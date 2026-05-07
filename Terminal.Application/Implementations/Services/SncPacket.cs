using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

/// <summary>
/// Пакет протокола SNC (реализация IPacket)
/// </summary>
public class SncPacket : IPacket
{
    public byte Cmd { get; set; }
    public ushort Offset { get; set; }
    public ushort Len => (ushort)(Data?.Length ?? 0);
    public byte[] Data { get; set; }
    
    public SncPacket()
    {
        Data = Array.Empty<byte>();
    }
    
    public SncPacket(byte cmd, ushort offset, byte[]? data)
    {
        Cmd = cmd;
        Offset = offset;
        Data = data ?? Array.Empty<byte>();
    }
    
    public ushort CalculateCrc16()
    {
        byte low = 0x28;
        byte high = 0x11;
        
        // Алгоритм CRC16 из C++ протокола
        CrcByte(ref low, ref high, Cmd);
        CrcByte(ref low, ref high, (byte)(Offset & 0xFF));
        CrcByte(ref low, ref high, (byte)(Offset >> 8));
        CrcByte(ref low, ref high, (byte)(Len & 0xFF));
        CrcByte(ref low, ref high, (byte)(Len >> 8));
        
        for (var i = 0; i < Len; i++)
            CrcByte(ref low, ref high, Data[i]);
        
        return (ushort)((high << 8) | low);
    }
    
    private static void CrcByte(ref byte low, ref byte high, byte byteToCrc)
    {
        var mask = 0x01;
        for (var i = 0; i < 8; i++)
        {
            var flag = (byteToCrc & mask) != 0;
            mask <<= 1;
            
            if ((low & 0x01) != 0)
                flag = !flag;
            
            if (flag)
            {
                high ^= 0x40;
                low ^= 0x02;
            }
            
            var flagC = (high & 0x01) != 0;
            high = (byte)(high >> 1);
            
            if (flag)
                high = (byte)(high | 0x80);
            
            low = (byte)(low >> 1);
            if (flagC)
                low = (byte)(low | 0x80);
        }
    }
}