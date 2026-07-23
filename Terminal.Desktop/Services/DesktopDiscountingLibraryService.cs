using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Desktop.Services;

/// <summary>
/// Desktop реализация интерфейса IDiscountingLibraryService - сервиса работы с библиотекой расчёта скидок.
/// </summary>
public class DesktopDiscountingLibraryService : IDiscountingLibraryService
{
    /// <summary>
    /// Название dll.
    /// </summary>
    private const string DllName = "Discounting.dll";

    /// <summary>
    /// Данные инициализации.
    /// Требуются для корректного завершения работы сервиса.
    /// </summary>
    private readonly IntPtr _initData;

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DesktopDiscountingLibraryService(string initParams = "")
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var sourceDirectory = Path.Combine(baseDirectory, "Libs", DllName);
        var targetDirectory = Path.Combine(baseDirectory, DllName);

        if (!File.Exists(targetDirectory))
            File.Move(sourceDirectory, targetDirectory);
        
        _initData = calculate_Initialize(initParams);
    }
    

    /// <inheritdoc/>
    public string Calculating(string szInputXml,
        string szInputLimitation,
        string szInputSchema,
        string szInputParam,
        byte[] pOutputXmlBuffer,
        int nOutputXmlBufferSize,
        ref uint pnReturnBytes)
    {
        calculate_Process(
            _initData,
            szInputXml, 
            szInputLimitation, 
            szInputSchema, 
            szInputParam, 
            pOutputXmlBuffer,
            nOutputXmlBufferSize, 
            ref pnReturnBytes);
        
        var resultString = Encoding.GetEncoding("windows-1251").GetString(pOutputXmlBuffer, 0, (int)pnReturnBytes);

        return resultString;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        calculate_Release(_initData);
    }
    
    /// <summary>
    /// Возвращает результат инициализации.
    /// </summary>
    /// <param name="szInputParam"></param>
    /// <returns>Результат для последующих вычислений. Должен существовать как singleton.</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    private static extern IntPtr calculate_Initialize(string szInputParam = "");
    
    /// <summary>
    /// Удаление результата инициализации. Всегда должен вызываться при выходе из приложения.
    /// </summary>
    /// <param name="pData">Результат инициализации.</param>
    /// <returns>Ничего.</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    private static extern IntPtr calculate_Release(IntPtr pData);
    
    /// <summary>
    /// Расчёт.
    /// </summary>
    /// <param name="pData">Результат инициализации.</param>
    /// <param name="szInputXml">(in.xml)</param>
    /// <param name="szInputLimitation">(limit.xml)</param>
    /// <param name="szInputSchema">(dsc)</param>
    /// <param name="szInputParam"></param>
    /// <param name="pOutputXmlBuffer">Выходной буфер. (out.xml должен получиться).</param>
    /// <param name="nOutputXmlBufferSize">Максимальный размер буфера.</param>
    /// <param name="pnReturnBytes">Сколько байт заполнено результатом.</param>
    /// <returns></returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    private static extern IntPtr calculate_Process(
        IntPtr pData,
        string szInputXml,
        string szInputLimitation,
        string szInputSchema,
        string szInputParam,
        byte[] pOutputXmlBuffer,
        int nOutputXmlBufferSize,
        ref uint pnReturnBytes);
}