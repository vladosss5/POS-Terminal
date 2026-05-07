using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class Packet
{
    public SncProtocolCode Cmd { get; set; }
    public ushort Offset { get; set; }
    public ushort Length { get; set; }
    public byte[] Data { get; set; } = [];
}