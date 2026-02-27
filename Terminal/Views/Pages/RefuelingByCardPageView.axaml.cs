using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
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
}