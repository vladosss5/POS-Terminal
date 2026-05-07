using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class MessageBoxService : IMessageBoxService
{
    /// <inheritdoc/>
    public async Task<ButtonResult> ShowMessageBoxAsync(string title,
        string message,
        ButtonEnum buttonEnum = ButtonEnum.Ok,
        Icon icon = Icon.None)
    {
        return await MessageBoxManager.GetMessageBoxStandard(title, message, buttonEnum, icon).ShowAsync();
    }
}