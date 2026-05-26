using Terminal.Core.Models;
using Terminal.Persistence.MainDB;

namespace Terminal.Application.Interfaces.DbEntitiesServices;

public interface IMainDbService
{
    public Task ExportDataFromMainDbAsync(TableToSendDto tableToSend, DataContext context);
}