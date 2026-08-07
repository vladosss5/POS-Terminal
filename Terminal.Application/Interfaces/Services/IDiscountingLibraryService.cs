namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с библиотекой расчёта скидок.
/// </summary>
public interface IDiscountingLibraryService : IDisposable
{
    /// <summary>
    /// Рассчитать скидку.
    /// </summary>
    /// <param name="szInputXml">(in.xml)</param>
    /// <param name="szInputLimitation">(limit.xml)</param>
    /// <param name="szInputSchema">(dsc)</param>
    /// <param name="szInputParam"></param>
    /// <param name="pOutputXmlBuffer">Выходной буфер. (out.xml должен получиться).</param>
    /// <param name="nOutputXmlBufferSize">Максимальный размер буфера.</param>
    /// <param name="pnReturnBytes">Сколько байт заполнено результатом.</param>
    /// <returns>Сериализованные xml данные расчётов.</returns>
    public string Calculating(string szInputXml,
        string szInputLimitation,
        string szInputSchema,
        string szInputParam,
        byte[] pOutputXmlBuffer,
        int nOutputXmlBufferSize,
        ref uint pnReturnBytes);
}