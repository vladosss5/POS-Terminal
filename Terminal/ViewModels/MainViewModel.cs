using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.ViewModels.Pages;

namespace Terminal.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private PageViewModelBase _currentPage;

    public MainViewModel(MainMenuPageViewModel mainMenuPageViewModel)
    {
        _currentPage = mainMenuPageViewModel;
    }
}