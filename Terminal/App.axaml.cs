using System;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Data.Context;
using Terminal.ViewModels;
using Terminal.ViewModels.NavigationService;
using Terminal.ViewModels.Pages;
using Terminal.Views;

namespace Terminal;

public partial class App : Avalonia.Application
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private static ILogger<App> _logger { get; set; }
    
    /// <summary>
    /// Св-во для получения зарегестрированных сервисов.
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
    /// Иницализация процессов после инита фреймфорка.
    /// </summary>
    public override async void OnFrameworkInitializationCompleted()
    {
        _logger = Services.GetRequiredService<ILogger<App>>();
        
        await InitializeDatabaseAsync();

        // Указание на первую открываемую страницу.
        var navigationService = Services.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<MainMenuPageViewModel>();

        var mainViewModel = Services.GetRequiredService<MainViewModel>();

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
            bool dataBaseIsClear;
            var factory = Services!.GetRequiredService<IDbContextFactory<DataContext>>();
            await using (var context = await factory.CreateDbContextAsync())
            {
                _logger.LogInformation("[DB] Start migration");
                await context.Database.MigrateAsync();
                _logger.LogInformation($"[DB] End migration");
                
                dataBaseIsClear = !await context.ResourceCodes.AnyAsync();
                _logger.LogInformation($"[DB] The SQL will have to be executed = {dataBaseIsClear}");
            }

            if (dataBaseIsClear)
            {
                string? sqlScript = await ReadSqlScriptFromResourceAsync();
                if (!string.IsNullOrEmpty(sqlScript))
                {
                    _logger.LogInformation($"[DB] Начато выполнение скриптов");
                    var sqlExecutor = Services.GetRequiredService<ISqlExecutor>();
                    var rowsAffected = await sqlExecutor.ExecuteNonQueryAsync(sqlScript);
                    _logger.LogInformation($"[DB] Скрипт выполнен успешно, затронуто строк: {rowsAffected}");
                }
                else
                {
                    _logger.LogError("[DB] Не удалось найти скрипт во встроенных ресурсах");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[DB] Критическая ошибка при инициализации БД: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Получение SQl из рессурсов проекта.
    /// </summary>
    private async Task<string?> ReadSqlScriptFromResourceAsync()
    {
        var assembly = typeof(App).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
    
        var sqlResource = resourceNames.FirstOrDefault(r => r.EndsWith("1_pos.terminal.sql")); // TODO: Переделать на выполнение множества скриптов.
    
        if (sqlResource == null)
        {
            _logger.LogError($"[DB] Скрипт не найден. Доступные ресурсы: {string.Join(", ", resourceNames)}");
            return null;
        }

        await using var stream = assembly.GetManifestResourceStream(sqlResource);
        using var reader = new StreamReader(stream!);
        return await reader.ReadToEndAsync();
    }
}