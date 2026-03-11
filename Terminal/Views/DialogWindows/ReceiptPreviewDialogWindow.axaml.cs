using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Terminal.ViewModels.DialogWindows;

namespace Terminal.Views.DialogWindows;

/// <summary>
/// Диалоговое окно для демонстрации чеков.
/// </summary>
public partial class ReceiptPreviewDialogWindow : Window
{
    private readonly TaskCompletionSource<bool> _tcs;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="receiptText"></param>
    public ReceiptPreviewDialogWindow(string receiptText)
    {
        InitializeComponent();
        
        _tcs = new TaskCompletionSource<bool>();
        DataContext = new ReceiptPreviewDialogViewModel(receiptText);
        
        Closed += (_, _) => _tcs.TrySetResult(false);
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