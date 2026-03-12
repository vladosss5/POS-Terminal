using Avalonia.Controls;
using Avalonia.Input;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

/// <summary>
/// Страница процесса продажи.
/// </summary>
public partial class SaleProcessPageView : UserControl
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SaleProcessPageView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Обработчик долгого нажатия.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnButtonHolding(object? sender, HoldingRoutedEventArgs e)
    {
        switch (e.HoldingState)
        {
            case HoldingState.Started:
                (DataContext as SaleProcessPageViewModel)!.AmountPreviewSetZero();
                break;
        }
    }

    /// <summary>
    /// Обработчик одноразового нажатия.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnButtonTapped(object? sender, TappedEventArgs e)
    {
        (DataContext as SaleProcessPageViewModel)!.DeleteLastCharFromPreview();
    }
}