using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Terminal.Services.MessageBoxService;

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