using Terminal.Core.Enums;

namespace Terminal.Persistence.TmsClient;

public interface ITmsClient
{
    public TmsConnectionStatus ConnectionStatus { get; }

    public Task AuthenticationAsync(string authData);
}