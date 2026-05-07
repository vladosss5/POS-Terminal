namespace Terminal.Core.Enums;

[Flags]
public enum PosFlags : ushort
{
    None    = 0x00,
    CRC32   = 0x01,
    GPRS    = 0x02,
}