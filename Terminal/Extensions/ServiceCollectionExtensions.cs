using Microsoft.Extensions.DependencyInjection;
using Terminal.ViewModels;

namespace Terminal.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainViewModel>();
    }
}