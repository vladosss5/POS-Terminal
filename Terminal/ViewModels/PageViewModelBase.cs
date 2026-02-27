using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.ViewModels.NavigationService;

namespace Terminal.ViewModels;

/// <summary>
/// Базовая модель страницы.
/// </summary>
public partial class PageViewModelBase : ViewModelBase
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<PageViewModelBase> _logger;
    
    /// <inheritdoc cref="INavigationService"/>
    private INavigationService? _navigationService;
    
    /// <summary>
    /// Сервис навигации (доступен только после активации страницы).
    /// </summary>
    protected INavigationService Navigation
    {
        get
        {
            if (_navigationService == null)
                _logger.LogError("NavigationService не инициализирован");
                    
            return _navigationService;
        }
    }
    
    /// <summary>
    /// Проверяет, инициализирована ли навигация
    /// </summary>
    protected bool IsNavigationInitialized => _navigationService != null;
    
    /// <summary>
    /// Заголовок страницы.
    /// </summary>
    [ObservableProperty]
    private string _title = string.Empty;

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    protected PageViewModelBase(
        ILogger<PageViewModelBase> logger)
    {
        _logger = logger;
    }

    
    /// <summary>
    /// Вызывается при активации страницы.
    /// </summary>
    public virtual void OnActivated(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }
    
    /// <summary>
    /// Вызывается при деактивации страницы.
    /// </summary>
    public void OnDeactivated() { }
    
    /// <summary>
    /// Команда для возврата на предыдущую страницу
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack()
    {
        if (IsNavigationInitialized)
        {
            Navigation.GoBack();
        }
    }
    
    /// <summary>
    /// Можно ли вернуться назад?
    /// </summary>
    /// <returns>Можно или не можно.</returns>
    protected bool CanGoBack() => IsNavigationInitialized && Navigation.CanGoBack;
}