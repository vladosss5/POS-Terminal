using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;

namespace Terminal.Application.Implementations.Services;

public class EncashmentService : IEncashmentService
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    private readonly IDbContextFactory<EventDbContext> _eventDbFactory;

    private readonly IEventDbService _eventDbService;

    private readonly IMainDbService _mainDbService;

    private readonly List<TableToSendDto> _tablesToSend;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public EncashmentService(
        IConfigurationService configurationService, 
        IDbContextFactory<DataContext> dbFactory, 
        IDbContextFactory<EventDbContext> eventDbFactory, 
        IMainDbService mainDbService, 
        IEventDbService eventDbService)
    {
        _dbFactory = dbFactory;
        _eventDbFactory = eventDbFactory;
        _mainDbService = mainDbService;
        _eventDbService = eventDbService;

        _tablesToSend = configurationService.GetTablesToSend();
    }
    
    public async Task EncashmentAsync()
    {
        var dbMain = await _dbFactory.CreateDbContextAsync();
        var dbEvent = await _eventDbFactory.CreateDbContextAsync();

        foreach (var tableToSend in _tablesToSend)
        {
            switch (tableToSend.DbName)
            {
                case "EventDb":
                    await _eventDbService.ExportDataFromTablesAsync(tableToSend, dbEvent);
                    break;
                
                default:
                    await _mainDbService.ExportDataFromMainDbAsync(tableToSend, dbMain);
                    break;
            }
        }
    }
}