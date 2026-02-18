using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.ViewModels.Pages;

namespace Terminal.ViewModels;

/// <summary>
/// Логика работы главного окна.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Отображаемая страница на еткущий момент.
    /// </summary>
    [ObservableProperty] private PageViewModelBase _currentPage;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainViewModel(MainMenuPageViewModel mainMenuPageViewModel)
    {
        _currentPage = mainMenuPageViewModel;
    }
}