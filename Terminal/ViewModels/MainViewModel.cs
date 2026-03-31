using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Application.Interfaces.Services;
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
    
    public bool ShowTopPanel { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainViewModel(
        INavigationService navigationService, 
        IDeviceInfoService deviceInfoService)
    {
        _navigationService = navigationService;
        
        _currentPage = _navigationService.CurrentPage;
        
        _navigationService.PageChanged += (s, page) => CurrentPage = page;

        var deviceManufacturer = deviceInfoService.DeviceInformation.Manufacturer;
        var deviceModel = deviceInfoService.DeviceInformation.Model;

        if (deviceManufacturer == "alps" && deviceModel == "S200")
            ShowTopPanel = true;
    }
}