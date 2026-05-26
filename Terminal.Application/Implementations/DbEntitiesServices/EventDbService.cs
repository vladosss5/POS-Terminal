using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Core.Models;
using Terminal.Persistence.EventDB;

namespace Terminal.Application.Implementations.DbEntitiesServices;

public class EventDbService : IEventDbService
{
    public async Task ExportDataFromTablesAsync(TableToSendDto tableToSend, EventDbContext dbEvent)
    {
        if (tableToSend.DbName != "EventDb")
            throw new Exception();
        
        switch (tableToSend.Name)
        {
            case "ProtocolFilingForm":
                await ExportTableFromEventDbAsync(
                    dbEvent.ProtocolFilingForms,
                    tableToSend,
                    p => p.ProtokolFillingFormKey
                );
                break;
            
            case "Incass":
                await ExportTableFromEventDbAsync(
                    dbEvent.Incasses,
                    tableToSend,
                    i => i.IncassKey
                );
                break;
            
            default:
                Console.WriteLine($"Unknown table in EventDb: {tableToSend.Name}");
                break;
        }
    }
    
    private async Task ExportTableFromEventDbAsync<T, TKey>(
        DbSet<T> dbSet, 
        TableToSendDto table, 
        Func<T, TKey> keySelector) 
        where T : class
        where TKey : struct, IComparable
    {
        throw new NotImplementedException();
    }
}