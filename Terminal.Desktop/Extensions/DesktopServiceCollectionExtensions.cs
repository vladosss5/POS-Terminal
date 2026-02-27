using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Desktop.Extensions;

/// <summary>
/// Расширение коллекции сервисов для Android.
/// </summary>
public static class DesktopServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов для Android.
    /// </summary>
    public static void AddDesktopServices(this IServiceCollection collection)
    {
        collection.AddScoped<IFileExplorer, FileExplorer>();
    }
}