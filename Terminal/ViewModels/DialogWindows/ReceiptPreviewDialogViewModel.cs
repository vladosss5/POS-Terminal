using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Terminal.ViewModels.DialogWindows;

public partial class ReceiptPreviewDialogViewModel : ViewModelBase
{
    [ObservableProperty] private string _receiptText;
    
    private readonly TaskCompletionSource<bool> _tcs;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="receiptText">Текст чека.</param>
    /// <param name="tcs"></param>
    public ReceiptPreviewDialogViewModel(
        string receiptText, 
        TaskCompletionSource<bool> tcs)
    {
        _receiptText = receiptText;
        _tcs = tcs;
    }
}