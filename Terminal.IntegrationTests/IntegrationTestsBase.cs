using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Application.Interfaces.Services;
using Terminal.Data.Context;
using Terminal.Services.NavigationService;

namespace Terminal.IntegrationTests;

public abstract class IntegrationTestsBase
{
    protected IServiceProvider? Services;
    protected IDbContextFactory<DataContext>? DbFactory;
    protected Mock<INavigationService>? NavigationMock;
    
    [OneTimeSetUp]
    public async Task Init()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.RegisterCommonServices();
        serviceCollection.AddDataContext();
        
        Services = serviceCollection.BuildServiceProvider();
        DbFactory = Services.GetRequiredService<IDbContextFactory<DataContext>>();
        NavigationMock = Services!.GetRequiredService<Mock<INavigationService>>();
        
        await EnsureDatabaseCreatedAndMigrated();

        await using var context = await DbFactory.CreateDbContextAsync();
        await ExecuteSqlAsync(context);
    }
    
    private async Task EnsureDatabaseCreatedAndMigrated()
    {
        await using var context = await DbFactory!.CreateDbContextAsync();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    private async Task ExecuteSqlAsync(DataContext context)
    {
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

        var sqlExecutor = Services!.GetRequiredService<ISqlExecutor>();
        
        foreach (var script in scripts)
            await sqlExecutor.ExecuteNonQueryAsync(script);
    }
}