using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class SellingProcessPageView : UserControl
{
    public SellingProcessPageView()
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
                (DataContext as SellingProcessPageViewModel)?.SetZero();
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
        (DataContext as SellingProcessPageViewModel)?.RemoveLastNumber();
    }
}