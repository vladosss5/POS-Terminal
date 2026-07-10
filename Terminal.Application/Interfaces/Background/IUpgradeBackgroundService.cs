namespace Terminal.Application.Interfaces.Background;

public interface IUpgradeBackgroundService
{
    public Task StartAutoUpgradeAsync();
}