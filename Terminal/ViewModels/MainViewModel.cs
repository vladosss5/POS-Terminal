using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Services.NavigationService;

namespace Terminal.ViewModels;

/// <summary>
/// Логика работы главного окна.
/// </summary>
public partial class MainViewModel : ViewModelBase, IStatusObserver
{
    /// <inheritdoc cref="INavigationService" />
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Отображаемая страница на текущий момент.
    /// </summary>
    [ObservableProperty] private PageViewModelBase _currentPage;
    
    /// <summary>
    /// Список отображаемых статусов.
    /// </summary>
    public ObservableCollection<Status> StatusList { get; set; } = [];
    
    

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainViewModel(
        INavigationService navigationService,
        IStatusNotifierService statusNotifierService)
    {
        _navigationService = navigationService;

        statusNotifierService.Attach(this);
        
        CurrentPage = _navigationService.CurrentPage;
        
        _navigationService.PageChanged += (s, page) => CurrentPage = page;
    }

    /// <inheritdoc/>
    public void UpdateStatuses(List<Status> statusList)
    {
        StatusList.Clear();
        Task.Delay(200);
        statusList.AddRange(statusList);
    }
}