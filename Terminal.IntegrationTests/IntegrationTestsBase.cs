using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Application.Implementations.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.ParamDb;
using Terminal.Data.MainDB;
using Terminal.Data.ParamDB;
using Terminal.Services.NavigationService;

namespace Terminal.IntegrationTests;

public abstract class IntegrationTestsBase
{
    private IServiceProvider? _services;
    
    protected IServiceScope? TestScope;
    protected IDbContextFactory<DataContext>? DbFactory;
    protected IDbContextFactory<ParamDbContext>? ParamDbFactory;
    protected readonly Mock<INavigationService> NavigationMock = new();
    protected readonly Mock<ICardReaderService> CardReaderMock = new();
    protected readonly Mock<IReceiptPrintService> ReceiptPrintMock = new();
    protected readonly Mock<ShiftService> MockShiftService = new();
    
    private readonly Mock<IMessageBoxService> _messageBoxMock = new();
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.RegisterCommonServices(
            NavigationMock, _messageBoxMock, 
            CardReaderMock, ReceiptPrintMock);
        
        serviceCollection.AddDataContext();
        
        _services = serviceCollection.BuildServiceProvider();
        DbFactory = _services.GetRequiredService<IDbContextFactory<DataContext>>();
        ParamDbFactory = _services.GetRequiredService<IDbContextFactory<ParamDbContext>>();
        
        await CreateDatabaseSchema();
        await SeedInitialData();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        TestScope = _services!.CreateScope();
        
        NavigationMock.Reset();
        _messageBoxMock.Reset();
        CardReaderMock.Reset();
        
        await CleanTestDataAsync();
    }
    
    [TearDown]
    public void TearDown()
    {
        TestScope?.Dispose();
        TestScope = null;
    }
    
    private async Task CreateDatabaseSchema()
    {
        await using var context = await DbFactory!.CreateDbContextAsync();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await using var paramContext = await ParamDbFactory!.CreateDbContextAsync();
        await paramContext.Database.EnsureDeletedAsync();
        await paramContext.Database.EnsureCreatedAsync();
    }

    private async Task CleanTestDataAsync()
    {
        await using var context = await DbFactory!.CreateDbContextAsync();
        
        await context.Database.ExecuteSqlRawAsync("DELETE FROM selling");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM users");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence");
    }

    private async Task SeedInitialData()
    {
        // Подгрузка данных для param db
        await using var paramContext = await ParamDbFactory!.CreateDbContextAsync();

        await paramContext.Params.AddRangeAsync(
        [
            new Param { Name = "SerialNO111", Value = "121" },
            new Param { Name = "IssuerId", Value = "973" }
        ]);
        await paramContext.SaveChangesAsync();
        
        // Подгрузка данных для основной БД
        await using var context = await DbFactory!.CreateDbContextAsync();
        
        List<string> scripts = [];

        if (!await context.ResourceCodes.AnyAsync())
        {
            var resourceTableFillingScript = await App.ReadSqlScriptFromResourceAsync("1_pos.terminal.sql");
                
            if (resourceTableFillingScript != null) 
                scripts.Add(resourceTableFillingScript);
        }

        if (!await context.Settings.AnyAsync())
        {
            var settingsTableFillingScript = await App.ReadSqlScriptFromResourceAsync("FillSettingsTable.sql");

            if (settingsTableFillingScript != null) 
                scripts.Add(settingsTableFillingScript);
        }

        if (!await context.Users.AnyAsync())
        {
            var usersTableFillingScript = await App.ReadSqlScriptFromResourceAsync("FillUsersTable.sql");

            if (usersTableFillingScript != null)
                scripts.Add(usersTableFillingScript);
        }

        var sqlExecutor = _services!.GetRequiredService<ISqlExecutor>();
        
        foreach (var script in scripts)
            await sqlExecutor.ExecuteNonQueryAsync(script);
    }
}