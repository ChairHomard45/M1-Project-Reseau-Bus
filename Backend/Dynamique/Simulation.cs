using Backend.Dynamique.Clock;
using Backend.Dynamique.Entities;
using Backend.Dynamique.Observer;
using Backend.Utils;
using Frontend.Observer;

using Frontend.Observer;

namespace Backend.Dynamique;

public class Simulation : IObserver<SimulationNotification>, IObservable<SimulationNotification>,
    IObserver<BusTimerNotification>, IObservable<Frontend.Observer.SimulationFrontNotification>
{
    public string Name { get; }
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }
    public TimeSpan CurrentTime => _currentTime;

    private readonly List<BusTimer> _busTimers = new();
    private readonly SimulationClock _clock;
    private TimeSpan _currentTime;

    private IDisposable? _clockSubscription;
    private readonly List<IObserver<SimulationNotification>> _observers = new();
    private readonly object _lock = new();

    private ITimeUnitStrategy _timeUnitStrategy;

    public Simulation(string name, TimeSpan startTime, TimeSpan endTime, ITimeUnitStrategy initialStrategy,
        int tickIntervalMs = 1000)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        _currentTime = startTime;

        _timeUnitStrategy = initialStrategy;
        _clock = new SimulationClock(startTime, tickIntervalMs, _timeUnitStrategy);

        _clockSubscription = _clock.Subscribe(this);

        this.Subscribe(_clock);
    }

    public void AddBus(Entities.Bus bus)
    {
        var timer = new BusTimer(bus, _timeUnitStrategy);
        _busTimers.Add(timer);
        timer.Subscribe(bus);
        _clock.Subscribe(timer);
    }

    public IDisposable Subscribe(IObserver<SimulationNotification> observer)
    {
        lock (_lock)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        return new Unsubscriber<SimulationNotification>(_observers, observer);
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

    public void Start()
    {
        NotifyObservers(new SimulationNotification(SimulationNotification.NotificationType.Start));
    }

    public void Stop()
    {
        NotifyObservers(new SimulationNotification(SimulationNotification.NotificationType.Stop));
        _clockSubscription?.Dispose();
        _clockSubscription = null;
    }

    public void ChangeTimeUnit(ITimeUnitStrategy newStrategy)
    {
        _timeUnitStrategy = newStrategy;
        _clock.UpdateStrategy(newStrategy);
        Console.WriteLine($"[SIMULATION] Time unit switched to: {newStrategy.GetType().Name}");
    }

    public void OnNext(SimulationNotification notification)
    {
        switch (notification.Type)
        {
            case SimulationNotification.NotificationType.Start:
                Console.WriteLine("[SIMULATION] Simulation commencer.");
                break;

            case SimulationNotification.NotificationType.Stop:
                Console.WriteLine("[SIMULATION] Simulation arreter.");
                break;

            case SimulationNotification.NotificationType.CurrentTime:
                if (notification.CurrentTime.HasValue)
                {
                    _currentTime = notification.CurrentTime.Value;
                    if (_currentTime >= EndTime)
                    {
                        Console.WriteLine("[SIMULATION] Atteint la fin de la simulation.");
                        Stop();
                    }
                }

                break;
        }
    }

    public void OnError(Exception error) => Console.WriteLine($"[SIMULATION] Clock error: {error.Message}");

    public void OnNext(BusTimerNotification value)
    {
        Console.WriteLine($"[SIMULATION] Bus {value.Bus.GetImmatriculation()} - {value.Type}");

        switch (value.Type)
        {
            case BusTimerNotification.NotificationType.MoveNext:
            case BusTimerNotification.NotificationType.ChangeStateToStop:
            case BusTimerNotification.NotificationType.ChangeStateToCirculation:
                value.Bus.OnNext(value);
                break;
        }
    }

    public void OnCompleted() => Console.WriteLine("[SIMULATION] Clock Compléter.");

    public IDisposable Subscribe(IObserver<SimulationFrontNotification> observer)
    {
        throw new NotImplementedException();
    }
}
