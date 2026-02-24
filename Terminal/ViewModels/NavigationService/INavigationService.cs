using System;

namespace Terminal.ViewModels.NavigationService;

public interface INavigationService
{
    /// <summary>
    /// Текущая страница
    /// </summary>
    PageViewModelBase CurrentPage { get; }
    
    /// <summary>
    /// Событие изменения страницы
    /// </summary>
    event EventHandler<PageViewModelBase>? PageChanged;
    
    /// <summary>
    /// Перейти на указанную страницу
    /// </summary>
    void NavigateTo<T>() where T : PageViewModelBase;
    
    /// <summary>
    /// Перейти на указанную страницу с параметрами
    /// </summary>
    void NavigateTo<T>(Action<T> configure) where T : PageViewModelBase;
    
    /// <summary>
    /// Вернуться назад (если есть история)
    /// </summary>
    void GoBack();
    
    /// <summary>
    /// Можно ли вернуться назад
    /// </summary>
    bool CanGoBack { get; }
}