using Avalonia.Controls;
using Avalonia.Input;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class RefuelingByCardPageView : UserControl
{
    public RefuelingByCardPageView()
    {
        InitializeComponent();
    }

    public void SetPaymentTypeCommand(PaymentTypes type)
    {
        (DataContext as RefuelingByCardPageViewModel).SetPaymentType(type);
    }

    public void SetFuelTypesCommand(ResourceCode type)
    {
        (DataContext as RefuelingByCardPageViewModel).SetFuelType(type);
    }
    
    private void OnButtonHolding(object? sender, HoldingRoutedEventArgs e)
    {
        switch (e.HoldingState)
        {
            case HoldingState.Started:
                (DataContext as RefuelingByCardPageViewModel).AmountPreviewSetZero();
                break;
        }
    }

    private void OnButtonTapped(object? sender, TappedEventArgs e)
    {
        (DataContext as RefuelingByCardPageViewModel).DeleteLastCharFromPreview();
    }
}