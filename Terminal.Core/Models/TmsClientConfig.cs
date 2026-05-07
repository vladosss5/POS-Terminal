using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class TmsClientConfig
{
    public string TerminalNumber { get; set; } = string.Empty;
    public int ShopKey { get; set; }
    public int Issuer { get; set; }
    public string Version { get; set; } = "1.0.0.0";
    public ConnectType ConnectionType { get; set; } = ConnectType.TcpIp;
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5786;
    public string ComPort { get; set; } = "COM1";
    public int BaudRate { get; set; } = 115200;
    public bool UseCrypto { get; set; } = true;
    public bool UseSynchro { get; set; } = true;
    public int TimeoutMs { get; set; } = 30000;
    public int PacketSizeKb { get; set; } = 4;
}