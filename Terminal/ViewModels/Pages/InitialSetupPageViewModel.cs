using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Terminal.Data.Context;

namespace Terminal.ViewModels.Pages;

public class InitialSetupPageViewModel : PageViewModelBase
{
    /// Фабрика <inheritdoc cref="ParamDbContext" />
    private readonly DbContextFactory<ParamDbContext> _dbContextFactory;

    public string IssuerNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }
    
    public string TerminalNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }
    
    public InitialSetupPageViewModel(
        ILogger<PageViewModelBase> logger, 
        DbContextFactory<ParamDbContext> dbContextFactory) 
        : base(logger)
    {
        _dbContextFactory = dbContextFactory;
        Title = "Первичная настройка";
    }
}