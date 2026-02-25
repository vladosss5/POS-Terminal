using System;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.UseCases;
using Terminal.Data.Context;
using Terminal.Extensions;
using Terminal.ViewModels;
using Terminal.ViewModels.NavigationService;
using Terminal.ViewModels.Pages;
using Terminal.Views;

namespace Terminal;

public partial class App : Avalonia.Application
{
    /// <summary>
    /// Св-во для получения зарегестрированных сервисов.
    /// </summary>
    public static IServiceProvider? Services { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
    
        collection.AddCommonServices();
        collection.AddDataContext();
    
        var services = collection.BuildServiceProvider();
        Services = services;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            await InitializeDatabaseAsync();
        }
    
        var navigationService = Services.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<MainMenuPageViewModel>();

        var mainViewModel = services.GetRequiredService<MainViewModel>();
    
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
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
            
            var factory = Services.GetRequiredService<IDbContextFactory<DataContext>>();
            await using (var context = await factory.CreateDbContextAsync())
            {
                await context.Database.MigrateAsync();
                dataBaseIsClear = !await context.ResourceCode.AnyAsync();
            }

            if (dataBaseIsClear)
            {
                var scriptHandler = Services.GetRequiredService<ExecuteSqlScriptsHandler>();
                string scriptPath = GetSqlScriptPath();

                if (File.Exists(scriptPath))
                {
                    var result = await scriptHandler.ExecuteFileAsync(scriptPath);
                    if (result.Success)
                    {
                        Console.WriteLine($"[DB] Скрипт {result.FileName} выполнен успешно, затронуто строк: {result.RowsAffected}");
                    }
                    else
                    {
                        Console.WriteLine($"[DB] Ошибка выполнения скрипта {result.FileName}: {result.ErrorMessage}");
                    }
                }
                else
                {
                    Console.WriteLine($"[DB] Файл скрипта не найден: {scriptPath}");
                }   
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB] Критическая ошибка при инициализации БД: {ex.Message}");
        }
    }
    
    private static string GetSqlScriptPath()
    {
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\.."));
        
        return Path.Combine(projectRoot, "SQL", "1_pos.terminal.sql");
    }
}