namespace Terminal.Core.Enums;

[Flags]
public enum AuthorizeFlags : byte
{
    NoCrypto    = 0x00,
    Crypto      = 0x01,
    SynchroTime = 0x02,
    GetConfig   = 0x04,
    CRC32       = 0x08,
}