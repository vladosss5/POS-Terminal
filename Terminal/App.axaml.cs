using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Data.Context;
using Terminal.Services.NavigationService;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;
using Terminal.Views;

namespace Terminal;

public partial class App : Avalonia.Application
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private static ILogger<App>? Logger { get; set; }
    
    /// <summary>
    /// Св-во для получения зарегистрированных сервисов.
    /// </summary>
    public static IServiceProvider? Services { get; set; }
    
    
    /// <summary>
    /// Отрисовать окна и страницы.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Инициализация процессов после инита фреймфорка.
    /// </summary>
    public override async void OnFrameworkInitializationCompleted()
    {
        if (Design.IsDesignMode) 
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }
        
        Logger = Services!.GetRequiredService<ILogger<App>>();
        
        await InitializeDatabaseAsync();

        await OpenFirstPage();

        var mainViewModel = Services!.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            desktopApp.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }


        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Метод отключения повторных проверок от Avalonia и CommunityToolkit.
    /// </summary>
    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    /// <summary>
    /// Инициализация БД.
    /// Запуск миграций.
    /// Запуск SQL скриптов.
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        try
        {
            var paramDbFactory = Services!.GetRequiredService<IDbContextFactory<ParamDbContext>>();
            await using var paramDb = await paramDbFactory.CreateDbContextAsync();
            await paramDb.Database.MigrateAsync();
            
            var factory = Services!.GetRequiredService<IDbContextFactory<DataContext>>();
            await using var context = await factory.CreateDbContextAsync();
            await context.Database.MigrateAsync();
            
            List<string> scripts = [];

            if (!await context.ResourceCodes.AnyAsync())
            {
                var resourceTableFillingScript = await ReadSqlScriptFromResourceAsync("1_pos.terminal.sql");
                
                if (resourceTableFillingScript != null) 
                    scripts.Add(resourceTableFillingScript);
            }

            if (!await context.Settings.AnyAsync())
            {
                var settingsTableFillingScript = await ReadSqlScriptFromResourceAsync("FillSettingsTable.sql");

                if (settingsTableFillingScript != null) 
                    scripts.Add(settingsTableFillingScript);
            }

            if (!await context.Users.AnyAsync())
            {
                var usersTableFillingScript = await ReadSqlScriptFromResourceAsync("FillUsersTable.sql");

                if (usersTableFillingScript != null)
                    scripts.Add(usersTableFillingScript);
            }

            var sqlExecutor = Services!.GetRequiredService<ISqlExecutor>();
            var rowsAffected = 0;
            
            Logger?.LogInformation($"[DB] Начато выполнение скриптов");
            foreach (var script in scripts)
            {
                rowsAffected = await sqlExecutor.ExecuteNonQueryAsync(script);
            }
            Logger?.LogInformation($"[DB] Скрипт выполнен успешно, затронуто строк: {rowsAffected}");
        }
        catch (Exception ex)
        {
            Logger?.LogError($"[DB] Критическая ошибка при инициализации БД: {ex.Message}");
        }
    }

    /// <summary>
    /// Открытие первой страницы в зависимости от настройки.
    /// </summary>
    private static async Task OpenFirstPage()
    {
        var navigationService = Services!.GetRequiredService<INavigationService>();
        var parameterService = Services!.GetRequiredService<IParameterService>();

        var isInstalled = await parameterService.GetValue(AppParameter.IsInstalled);

        if (isInstalled == "1")
        {
            navigationService.NavigateTo<OpenShiftPageViewModel>();
            return;
        }
        
        navigationService.NavigateTo<InitialSetupPageViewModel>();
    }
    
    /// <summary>
    /// Получение SQl из ресурсов проекта.
    /// </summary>
    public static async Task<string?> ReadSqlScriptFromResourceAsync(string scriptName)
    {
        var assembly = typeof(App).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
    
        var sqlResource = resourceNames.FirstOrDefault(r => r.EndsWith(scriptName));
    
        if (sqlResource == null)
        {
            Logger?.LogError($"[DB] Скрипт не найден. Доступные ресурсы: {string.Join(", ", resourceNames)}");
            return null;
        }

        await using var stream = assembly.GetManifestResourceStream(sqlResource);
        using var reader = new StreamReader(stream!);
        return await reader.ReadToEndAsync();
    }
}