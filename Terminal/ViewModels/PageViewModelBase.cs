namespace Terminal.ViewModels;

/// <summary>
/// Базовая модель страницы.
/// </summary>
public class PageViewModelBase : ViewModelBase
{
    public string? Title { get; init; }
}