namespace Terminal.Application.Interfaces.Services;

public interface IPacket
{
    /// <summary>
    /// Команда протокола (тип пакета)
    /// </summary>
    byte Cmd { get; }
    
    /// <summary>
    /// Смещение (номер пакета при фрагментации)
    /// </summary>
    ushort Offset { get; }
    
    /// <summary>
    /// Длина данных
    /// </summary>
    ushort Len { get; }
    
    /// <summary>
    /// Данные пакета
    /// </summary>
    byte[] Data { get; }
    
    /// <summary>
    /// Вычисление контрольной суммы CRC16
    /// </summary>
    ushort CalculateCrc16();
}