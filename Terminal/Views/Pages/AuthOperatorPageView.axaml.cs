using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class AuthOperatorPageView : UserControl
{
    public AuthOperatorPageView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Обработчик долгого нажатия.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnButtonHolding(object? sender, HoldingRoutedEventArgs e)
    {
        switch (e.HoldingState)
        {
            case HoldingState.Started:
                (DataContext as AuthOperatorPageViewModel)!.ClearPassword();
                break;
        }
    }

    /// <summary>
    /// Обработчик одноразового нажатия.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnButtonTapped(object? sender, TappedEventArgs e)
    {
        (DataContext as AuthOperatorPageViewModel)!.RemoveLastChar();
    }
}