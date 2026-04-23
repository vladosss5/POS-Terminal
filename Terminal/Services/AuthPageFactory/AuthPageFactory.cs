using System;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Dtos;
using Terminal.ViewModels.Pages;

namespace Terminal.Services.AuthPageFactory;

/// <inheritdoc/>
public class AuthPageFactory : IAuthPageFactory
{
    /// <summary>
    /// Механизм извлечения сервисов.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceProvider">Механизм извлечения сервисов.</param>
    public AuthPageFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public AuthOperatorPageViewModel Create(AuthNavigationParameters parameters) 
        => ActivatorUtilities.CreateInstance<AuthOperatorPageViewModel>(_serviceProvider, parameters);
}