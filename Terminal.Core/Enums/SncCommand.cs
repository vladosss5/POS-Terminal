namespace Terminal.Core.Enums;

public enum SncCommand : byte
{
    StartPacket     = 0x01,
    SendFile        = 0x02,
    DataI           = 0x03,
    EndFile         = 0x04,
    Authorize       = 0x05,
    Receive         = 0x06,
    AuthorizeKey    = 0x09,
    EndDialog       = 0x11,
    SendTable       = 0x12,
    ReceiveUpdate   = 0x13,
    EndUpdate       = 0x14,
    Synchro         = 0x15,
    SendFileHeader  = 0x33
}