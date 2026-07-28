using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.Interfaces.Services;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.Services.NavigationService;

/// <summary>
/// Реализация сервиса навигации.
/// </summary>
public class NavigationService : INavigationService
{
    ///<inheritdoc cref="IServiceProvider"/>
    private readonly IServiceProvider _serviceProvider;

    ///<inheritdoc cref="IParameterService"/>
    private readonly IParameterService _parameterService;
    
    /// <summary>
    /// Стек истории открытия страниц.
    /// </summary>
    private readonly Stack<PageViewModelBase> _history = new();
    
    /// <summary>
    /// Текущая открытая страница.
    /// </summary>
    private PageViewModelBase? _currentPage;
    
    ///<inheritdoc/>
    public event EventHandler<PageViewModelBase>? PageChanged;
    
    /// <summary>
    /// Публичное св-во для управления текущей страницей.
    /// </summary>
    public PageViewModelBase CurrentPage 
    { 
        get => _currentPage!;
        private set
        {
            _currentPage = value;
            PageChanged?.Invoke(this, value);
        }
    }
    
    /// <summary>
    /// Проверка: есть ли в истории открытия страниц предшественник.
    /// </summary>
    public bool CanGoBack => _history.Count > 0;
    
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public NavigationService(
        IServiceProvider serviceProvider, 
        IParameterService parameterService)
    {
        _serviceProvider = serviceProvider;
        _parameterService = parameterService;

        _ = OpenFirstPageAsync();
    }
    
    ///<inheritdoc/>
    public void NavigateTo<T>() where T : PageViewModelBase
    {
        var page = _serviceProvider.GetRequiredService<T>();
        NavigateToPage(page);
    }

    ///<inheritdoc/>
    public void NavigateToInstancePage(PageViewModelBase page)
    {
        NavigateToPage(page);
    }
    
    ///<inheritdoc/>
    public void NavigateTo<T>(Action<T> configure) where T : PageViewModelBase
    {
        var page = _serviceProvider.GetRequiredService<T>();
        configure(page);
        NavigateToPage(page);
    }
    
    ///<inheritdoc/>
    public void GoBack()
    {
        if (_history.Count > 0)
        {
            _currentPage?.OnDeactivated();
            
            var previousPage = _history.Pop();
            previousPage.OnActivated(this);
            CurrentPage = previousPage;
        }
    }
    
    /// <summary>
    /// Переключение страницы.
    /// </summary>
    /// <param name="page">Страница которую нужно отобразить.</param>
    private void NavigateToPage(PageViewModelBase page)
    {
        page.OnActivated(this);
    
        if (_currentPage != null)
        {
            _currentPage.OnDeactivated();
            _history.Push(_currentPage);
        }

        CurrentPage = page;
    }

    /// <summary>
    /// Открытие первой страницы в зависимости от настройки.
    /// </summary>
    private async Task OpenFirstPageAsync()
    {
        bool isInstalled;
        
        try
        {
            isInstalled = await _parameterService.CheckSetupComplete();
        }
        catch (Exception e)
        {
            isInstalled = false;
        }
        
        if (isInstalled)
            NavigateTo<OpenShiftPageViewModel>();
        else
            NavigateTo<InitialSetupPageViewModel>();
    }
}