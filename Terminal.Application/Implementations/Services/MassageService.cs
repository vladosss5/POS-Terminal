using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

public class MassageService : IMassageService
{
    public string Message { get; private set; }

    private List<IMessageObserver> _observers = [];
    
    public void Attach(IMessageObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IMessageObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.UpdateMessage(this);
        }
    }

    public void ShowMessage(string message)
    {
        Message = message;
        Notify();
    }
}