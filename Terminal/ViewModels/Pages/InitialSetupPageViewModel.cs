using System.Threading.Tasks;
using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Core.ParamDbEntities;
using Terminal.Data.Context;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы первичной настройки терминала.
/// </summary>
public class InitialSetupPageViewModel : PageViewModelBase
{
    /// Фабрика <inheritdoc cref="ParamDbContext" />
    private readonly IDbContextFactory<ParamDbContext> _dbContextFactory;

    /// <summary>
    /// Номер эмитента.
    /// </summary>
    public string IssuerNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Номер терминала.
    /// </summary>
    public string TerminalNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Безопасная зона для интерфейсов при открытии клавиатуры.
    /// </summary>
    public Thickness SafeArea
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public InitialSetupPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<ParamDbContext> dbContextFactory) 
        : base(logger)
    {
        _dbContextFactory = dbContextFactory;
        Title = "Первичная настройка";
    }

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public async Task SaveSetupAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        await db.Params.AddRangeAsync([
            new Param { Name = "IsInstalled", Value = "1" },
            new Param { Name = "IssuerId", Value = IssuerNumber },
            new Param { Name = "SerialNO111", Value = TerminalNumber}
        ]);

        await db.SaveChangesAsync();
        
        Navigation.NavigateTo<OpenShiftPageViewModel>();
    }
}