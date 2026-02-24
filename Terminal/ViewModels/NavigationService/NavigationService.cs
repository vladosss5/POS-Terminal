using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Terminal.ViewModels.NavigationService;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<PageViewModelBase> _history = new();
    
    private PageViewModelBase? _currentPage;
    
    public event EventHandler<PageViewModelBase>? PageChanged;
    
    public PageViewModelBase CurrentPage 
    { 
        get => _currentPage!;
        private set
        {
            _currentPage = value;
            PageChanged?.Invoke(this, value);
        }
    }
    
    public bool CanGoBack => _history.Count > 0;
    
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void NavigateTo<T>() where T : PageViewModelBase
    {
        var page = _serviceProvider.GetRequiredService<T>();
        NavigateToPage(page);
    }
    
    public void NavigateTo<T>(Action<T> configure) where T : PageViewModelBase
    {
        var page = _serviceProvider.GetRequiredService<T>();
        configure(page);
        NavigateToPage(page);
    }
    
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
}