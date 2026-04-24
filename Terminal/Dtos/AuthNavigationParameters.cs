using System;
using Terminal.ViewModels;

namespace Terminal.Dtos;

/// <summary>
/// Параметры для создания экземпляра страницы аутентификации оператора.
/// </summary>
public class AuthNavigationParameters
{
    /// <summary>
    /// Тип страницы для перехода при успешной аутентификации.
    /// </summary>
    public Type SuccessPageType { get; init; } = null!;
    
    /// <summary>
    /// Тип страницы для перехода при ошибке/отмене.
    /// </summary>
    public Type? FailurePageType { get; init; }
    
    /// <summary>
    /// Действие для конфигурации страницы успеха.
    /// </summary>
    public Action<PageViewModelBase>? ConfigureSuccessPage { get; set; }
    
    /// <summary>
    /// Вернуться назад при отмене (если true — игнорирует FailurePageType).
    /// </summary>
    public bool GoBackOnCancel { get; init; } = true;
}