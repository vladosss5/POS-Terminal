using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Core.DbEntities;
using Terminal.Data.Context;

namespace Terminal.ViewModels.Pages;

public class ResourcePageViewModel : PageViewModelBase
{
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    public ObservableCollection<ResourceCode> Resources
    {
        get; 
        set => SetProperty(ref field, value);
    }
        
    public ResourcePageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory) 
        : base(logger)
    {
        _dbFactory = dbFactory;

        _ = LoadData();
    }

    private async Task LoadData()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var resources = await db.ResourceCodes.ToListAsync();
        Resources = new ObservableCollection<ResourceCode>(resources);
    }
}