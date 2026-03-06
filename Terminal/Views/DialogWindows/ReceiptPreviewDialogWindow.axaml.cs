using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Terminal.ViewModels.DialogWindows;

namespace Terminal.Views.DialogWindows;

public partial class ReceiptPreviewDialogWindow : Window
{
    private readonly TaskCompletionSource<bool> _tcs;
    
    public ReceiptPreviewDialogWindow(string receiptText)
    {
        InitializeComponent();
        
        _tcs = new TaskCompletionSource<bool>();
        DataContext = new ReceiptPreviewDialogViewModel(receiptText, _tcs);
        
        Closed += (s, e) => _tcs.TrySetResult(false);
    }
    
    /// <summary>
    /// Показывает диалог как модальное окно (для Desktop).
    /// </summary>
    public Task<bool> ShowModalDialog(Window owner)
    {
        ShowDialog(owner);
        return _tcs.Task;
    }

    /// <summary>
    /// Показывает диалог как обычное окно (для Android).
    /// </summary>
    public Task<bool> ShowModalDialog()
    {
        Show();
        return _tcs.Task;
    }
}