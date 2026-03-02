namespace Terminal.Core.Enums;

public enum PrinterStatus
{
    Ready = 0x00,
    PaperEnded = 0xF0,
    HardwareError = 0xF2,
    Overheat = 0xF3,
    BufferOverflow = 0xF5,
    LowVoltage = 0xE1,
    PaperJam = 0xEE,
    Busy = 0xF7,
    Unknown = -1
}