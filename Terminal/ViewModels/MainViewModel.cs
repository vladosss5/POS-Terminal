using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Interfaces;
using Terminal.Services.NavigationService;

namespace Terminal.ViewModels;

/// <summary>
/// Логика работы главного окна.
/// </summary>
public partial class MainViewModel : ViewModelBase, IStatusObserver, IPopupObserver
{
    /// <inheritdoc cref="INavigationService" />
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Отображаемая страница на текущий момент.
    /// </summary>
    [ObservableProperty]
    public partial PageViewModelBase CurrentPage { get; set; }

    /// <summary>
    /// Список отображаемых статусов.
    /// </summary>
    public ObservableCollection<Status> StatusList { get; set; } = [];
    
    /// <summary>
    /// Всплывающие уведомления.
    /// </summary>
    public ObservableCollection<Popup> Popups { get; set; } = [];

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainViewModel(
        INavigationService navigationService,
        IStatusNotifierService statusNotifierService,
        IPopupService popupService)
    {
        _navigationService = navigationService;

        statusNotifierService.Attach(this);
        popupService.Attach(this);
        
        CurrentPage = _navigationService.CurrentPage;
        _navigationService.PageChanged += (s, page) => CurrentPage = page;
    }

    /// <inheritdoc/>
    public void UpdateStatuses(List<Status> statusList)
    {
        StatusList.Clear();
        
        foreach (var status in statusList)
            StatusList.Add(status);
    }

    /// <inheritdoc/>
    public void OnPopupChanged(List<Popup> popups)
    {
        Popups.Clear();
        
        foreach (var popup in popups)
            Popups.Add(popup);
    }
}