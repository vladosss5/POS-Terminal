using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Core.Enums;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class RefuelingByCardPageView : UserControl
{
    public RefuelingByCardPageView()
    {
        InitializeComponent();
        if (App.Services != null)
            DataContext = App.Services.GetRequiredService<RefuelingByCardPageViewModel>();
    }

    public void SetPaymentTypeCommand(PaymentTypes type)
    {
        (DataContext as RefuelingByCardPageViewModel).SetPaymentType(type);
    }

    public void SetFuelTypesCommand(FuelTypes type)
    {
        (DataContext as RefuelingByCardPageViewModel).SetFuelType(type.ToString());
    }
}