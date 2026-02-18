using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class MainMenuPageView : UserControl
{
    public MainMenuPageView()
    {
        InitializeComponent();
        if (App.Services != null)
            DataContext = App.Services.GetRequiredService<MainMenuPageViewModel>();
    }
}