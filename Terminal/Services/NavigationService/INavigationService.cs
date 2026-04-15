using System;
using Terminal.ViewModels;

namespace Terminal.Services.NavigationService;

/// <summary>
/// Сервис навигации между страницами, окнами.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Текущая страница
    /// </summary>
    public PageViewModelBase CurrentPage { get; }
    
    /// <summary>
    /// Событие изменения страницы
    /// </summary>
    public event EventHandler<PageViewModelBase>? PageChanged;
    
    /// <summary>
    /// Перейти на указанную страницу
    /// </summary>
    public void NavigateTo<T>() where T : PageViewModelBase;
    
    /// <summary>
    /// Перейти на указанную страницу с параметрами
    /// </summary>
    public void NavigateTo<T>(Action<T> configure) where T : PageViewModelBase;

    /// <summary>
    /// Перейти к конкретному экземпляру страницы.
    /// </summary>
    /// <param name="page"></param>
    public void NavigateToInstancePage(PageViewModelBase page);
    
    /// <summary>
    /// Вернуться назад (если есть история)
    /// </summary>
    public void GoBack();
    
    /// <summary>
    /// Можно ли вернуться назад
    /// </summary>
    public bool CanGoBack { get; }
}