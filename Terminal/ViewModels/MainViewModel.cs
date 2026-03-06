using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Services.NavigationService;

namespace Terminal.ViewModels;

/// <summary>
/// Логика работы главного окна.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    
    /// <summary>
    /// Отображаемая страница на еткущий момент.
    /// </summary>
    [ObservableProperty] private PageViewModelBase _currentPage;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        
        _currentPage = _navigationService.CurrentPage;
        
        _navigationService.PageChanged += (s, page) => CurrentPage = page;
    }
}