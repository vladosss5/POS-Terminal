namespace Terminal.Application.Interfaces.Services;

public interface IMassageService
{
    public void Attach(IMessageObserver observer);

    public void Detach(IMessageObserver observer);

    public void Notify();

    public void ShowMessage(string message);
}