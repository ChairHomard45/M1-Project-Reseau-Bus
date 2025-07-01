namespace Backend.Dynamique.ObserverObservablePattern;

internal sealed class Unsubscriber<TNotification> : IDisposable
{
  private readonly IObserver<TNotification> _observer;
  private readonly ISet<IObserver<TNotification>> _observers;

  internal Unsubscriber(
    ISet<IObserver<TNotification>> observers,
    IObserver<TNotification> observer)
  {
    (_observers, _observer) =
      (observers, observer);
  }

  public void Dispose()
  {
    _observers.Remove(_observer);
  }
}