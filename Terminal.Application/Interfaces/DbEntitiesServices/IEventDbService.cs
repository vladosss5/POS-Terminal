using Terminal.Core.Models;
using Terminal.Persistence.EventDB;

namespace Terminal.Application.Interfaces.DbEntitiesServices;

public interface IEventDbService
{
    public Task ExportDataFromTablesAsync(TableToSendDto tableToSend, EventDbContext dbEvent);
}