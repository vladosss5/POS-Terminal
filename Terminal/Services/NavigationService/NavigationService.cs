using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Interfaces;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.Services.NavigationService;

/// <summary>
/// Реализация сервиса навигации.
/// </summary>
public class NavigationService : INavigationService
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILogger<NavigationService> _logger;
    
    ///<inheritdoc cref="IServiceProvider"/>
    private readonly IServiceProvider _serviceProvider;

    ///<inheritdoc cref="IParameterService"/>
    private readonly IParameterService _parameterService;

    ///<inheritdoc cref="IPopupService"/>
    private readonly IPopupService _popupService;
    
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
        ILogger<NavigationService> logger, 
        IParameterService parameterService, 
        IPopupService popupService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _parameterService = parameterService;
        _popupService = popupService;

        _ = OpenFirstPageAsync();
    }
    
    ///<inheritdoc/>
    public void NavigateTo<T>() where T : PageViewModelBase
    {
        try
        {
            var page = _serviceProvider.GetRequiredService<T>();
            NavigateToPage(page);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Ошибка перехода: {e.Message}");
        }
    }

    ///<inheritdoc/>
    public void NavigateToInstancePage(PageViewModelBase page)
    {
        try
        {
            NavigateToPage(page);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Ошибка перехода: {e.Message}");
        }
    }
    
    ///<inheritdoc/>
    public void NavigateTo<T>(Action<T> configure) where T : PageViewModelBase
    {
        try
        {
            var page = _serviceProvider.GetRequiredService<T>();
            configure(page);
            NavigateToPage(page);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Ошибка перехода: {e.Message}");
        }
    }
    
    ///<inheritdoc/>
    public void GoBack()
    {
        try
        {
            if (_history.Count <= 0) 
                return;
            
            _currentPage?.OnDeactivated();
                
            var previousPage = _history.Pop();
            previousPage.OnActivated(this);
            CurrentPage = previousPage;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Ошибка перехода: {e.Message}");
        }
    }
    
    /// <summary>
    /// Переключение страницы.
    /// </summary>
    /// <param name="page">Страница которую нужно отобразить.</param>
    private void NavigateToPage(PageViewModelBase page)
    {
        try
        {
            page.OnActivated(this);
        
            if (_currentPage != null)
            {
                _currentPage.OnDeactivated();
                _history.Push(_currentPage);
            }

            CurrentPage = page;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Ошибка перехода: {e.Message}");
        }
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
            _logger.LogError(e.Message, e.InnerException);
            isInstalled = false;
        }
        
        if (isInstalled)
            NavigateTo<OpenShiftPageViewModel>();
        else
            NavigateTo<InitialSetupPageViewModel>();
    }
}