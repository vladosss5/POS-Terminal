using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Terminal.ViewModels.NavigationService;

namespace Terminal.ViewModels;

/// <summary>
/// Базовая модель страницы.
/// </summary>
public partial class PageViewModelBase : ViewModelBase
{
    private INavigationService? _navigationService;
    
    /// <summary>
    /// Сервис навигации (доступен только после активации страницы).
    /// </summary>
    protected INavigationService Navigation
    {
        get
        {
            if (_navigationService == null)
            {
                throw new InvalidOperationException(
                    "NavigationService not initialized. " +
                    "Make sure the page is activated through NavigationService.NavigateTo() " +
                    "and not created manually with 'new'."
                );
            }
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
    /// Вызывается при активации страницы.
    /// </summary>
    public virtual void OnActivated(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }
    
    /// <summary>
    /// Вызывается при деактивации страницы.
    /// </summary>
    public virtual void OnDeactivated() { }
    
    /// <summary>
    /// Команда для возврата на предыдущую страницу
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoBack))]
    protected void GoBack()
    {
        if (IsNavigationInitialized)
        {
            Navigation.GoBack();
        }
    }
    
    protected bool CanGoBack()
    {
        return IsNavigationInitialized && Navigation.CanGoBack;
    }
}