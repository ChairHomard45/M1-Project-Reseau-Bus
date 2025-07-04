using Backend.Dynamique.Observer;
using Timer = System.Timers.Timer;

namespace Backend.Dynamique.Clock;

public class SimulationClock : IObservable<SimulationNotification>, IObserver<SimulationNotification>
{
    private readonly List<IObserver<SimulationNotification>> _observers = new();
    private readonly object _lock = new();

    private Timer? _timer;
    private TimeSpan _currentTime;
    private ITimeUnitStrategy _strategy;

    public SimulationClock(TimeSpan startTime, int tickIntervalMs, ITimeUnitStrategy strategy)
    {
        _currentTime = startTime;
        _strategy = strategy;

        _timer = new Timer(tickIntervalMs);
        _timer.Elapsed += (s, e) => Tick();
        _timer.AutoReset = true;
    }

    public void UpdateStrategy(ITimeUnitStrategy newStrategy)
    {
        _strategy = newStrategy;
    }

    // IObservable implementation
    public IDisposable Subscribe(IObserver<SimulationNotification> observer)
    {
        lock (_lock)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }
        return new Unsubscriber(_observers, observer, _lock);
    }

    private void NotifyObservers(SimulationNotification notification)
    {
        List<IObserver<SimulationNotification>> observersCopy;
        lock (_lock)
        {
            observersCopy = new List<IObserver<SimulationNotification>>(_observers);
        }

        foreach (var obs in observersCopy)
        {
            obs.OnNext(notification);
        }
    }

    private void Tick()
    {
        _currentTime = _strategy.AdvanceSimulationTime(_currentTime);
        NotifyObservers(new SimulationNotification(SimulationNotification.NotificationType.CurrentTime, _currentTime));
    }

    // IObserver implementation — reacts to commands from Simulation
    public void OnNext(SimulationNotification notification)
    {
        switch (notification.Type)
        {
            case SimulationNotification.NotificationType.Start:
                _timer?.Start();
                NotifyObservers(notification);
                break;

            case SimulationNotification.NotificationType.Stop:
                _timer?.Stop();
                NotifyObservers(notification);
                break;

            case SimulationNotification.NotificationType.CurrentTime:
                break;
        }
    }

    public void OnError(Exception error)
    {
    }

    public void OnCompleted()
    {
        _timer?.Stop();
        NotifyObservers(new SimulationNotification(SimulationNotification.NotificationType.Stop));
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<SimulationNotification>> _observers;
        private readonly IObserver<SimulationNotification> _observer;
        private readonly object _lock;

        public Unsubscriber(List<IObserver<SimulationNotification>> observers, IObserver<SimulationNotification> observer, object lockObj)
        {
            _observers = observers;
            _observer = observer;
            _lock = lockObj;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
