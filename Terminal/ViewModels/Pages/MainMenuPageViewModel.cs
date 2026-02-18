using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы главного меню.
/// </summary>
public partial class MainMenuPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel()
    {
        Title = "Главная";
    }
}