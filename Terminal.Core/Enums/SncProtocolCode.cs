namespace Terminal.Core.Enums;

public enum SncProtocolCode : byte
{
    StartPacket = 0x01,
    SendFile = 0x02,
    DataI = 0x03,
    EndFile = 0x04,
    Authorize = 0x05,
    Receive = 0x06,
    CardInfo = 0x07,
    CardInfoReq = 0x08,
    AuthorizeKey = 0x09,
    Update = 0x10,
    EndDialog = 0x11,
    SendTable = 0x12,
    ReceiveUpdate = 0x13,
    EndUpdate = 0x14,
    Synchro = 0x15,
    SendFileHeader = 0x33,
    ProtocolTransmit = 0x34,
    ProtocolSndFile = 0x35,
    ProtocolGetFile = 0x36,
    Responce = 0x55,
}