using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Terminal.ViewModels.DialogWindows;

/// <summary>
/// Диалоговое окно для отображения чеков.
/// </summary>
public partial class ReceiptPreviewDialogViewModel : ViewModelBase
{
    /// <summary>
    /// Текст чека.
    /// </summary>
    [ObservableProperty]
    public partial string ReceiptText { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="receiptText">Текст чека.</param>
    public ReceiptPreviewDialogViewModel(string receiptText)
    {
        ReceiptText = receiptText;
    }
}