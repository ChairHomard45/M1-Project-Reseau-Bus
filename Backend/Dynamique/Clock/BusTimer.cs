using Backend.Dynamique.Entities;
using Backend.Dynamique.Observer;
using Backend.Reseau;
using Backend.Reseau.Elements;
using Backend.Utils;

namespace Backend.Dynamique.Clock;

public class BusTimer : IObserver<SimulationNotification>, IObservable<BusTimerNotification>
{
  private List<IObserver<BusTimerNotification>> _observers = new();
  private IDisposable? _unsubscriber;
  private readonly Bus _bus;

  private TimeSpan? _intervalStartTime;
  private TimeRange? _currentTimeRange;

  private bool _isAtStop = true;
  private TimeSpan _expectedDuration;

  private static readonly Random Random = new Random();

  private readonly ITimeUnitStrategy _timeUnitStrategy;

  public BusTimer(Bus bus, ITimeUnitStrategy timeUnitStrategy)
  {
    _bus = bus;
    _timeUnitStrategy = timeUnitStrategy;
  }
  
  private void Notify(BusTimerNotification notification)
  {
    foreach (var observer in _observers)
      observer.OnNext(notification);
  }


  public IDisposable Subscribe(IObserver<BusTimerNotification> observer)
  {
    if (!_observers.Contains(observer))
      _observers.Add(observer);
    return new Unsubscriber<BusTimerNotification>(_observers, observer);
  }
  
  public void OnNext(SimulationNotification notification)
  {
    if (notification.Type == SimulationNotification.NotificationType.CurrentTime && notification.CurrentTime.HasValue)
    {
      TimeSpan currentTime = notification.CurrentTime.Value;
      var line = _bus.getOnline();
      int index = _bus.getIndexPositionOnLine();
      var currentStop = line.GetStopByIndex(index);

      _currentTimeRange = currentStop.GetServiceTimes().Keys.FirstOrDefault(tr => tr.Contains(currentTime));

      if (_currentTimeRange == null)
      {
        return;
      }

      if (_intervalStartTime == null)
      {
        _intervalStartTime = currentTime;

        if (_isAtStop)
        {
          if (currentStop.GetServiceTimes().TryGetValue(_currentTimeRange, out var stats))
          {
            var randomized = CalculateRandomizedTime(stats.MeanTime, stats.StdDeviation);
            _expectedDuration = TimeSpan.FromSeconds(_timeUnitStrategy.ConvertToSeconds(randomized));
          }
          else
          {
            _expectedDuration = TimeSpan.Zero;
          }
        }
        else
        {
          Connection? connection = GetCurrentConnection(line, index, _bus.GetDirection());
          if (connection != null)
          {
            float travelMinutes = connection.CalculateRandomizedTravelTime(currentTime);
            _expectedDuration = TimeSpan.FromSeconds(_timeUnitStrategy.ConvertToSeconds(travelMinutes));
          }
          else
          {
            _expectedDuration = TimeSpan.Zero;
          }
        }
      }
      else
      {
        var elapsed = currentTime - _intervalStartTime.Value;
        if (elapsed >= _expectedDuration)
        {
          if (_isAtStop)
          {
            _isAtStop = false;
            Notify(new BusTimerNotification(_bus, BusTimerNotification.NotificationType.ChangeStateToCirculation));
          }
          else
          {
            _isAtStop = true;
            Notify(new BusTimerNotification(_bus, BusTimerNotification.NotificationType.MoveNext));
            Notify(new BusTimerNotification(_bus, BusTimerNotification.NotificationType.ChangeStateToStop));
          }
          _intervalStartTime = null;
        }
      }
    }
  }

  private Connection? GetCurrentConnection(BusLine line, int index, bool direction)
  {
    if (direction)
    {
      if (index >= 0 && index < line.CountStops() - 1)
      {
        var current = line.GetStopByIndex(index);
        var next = line.GetStopByIndex(index + 1);
        return current.Connections?.FirstOrDefault(c => c != null && (c.GetStopA() == next || c.GetStopB() == next));
      }
    }
    else
    {
      if (index > 0 && index < line.CountStops())
      {
        var current = line.GetStopByIndex(index);
        var prev = line.GetStopByIndex(index - 1);
        return current.Connections?.FirstOrDefault(c => c != null && (c.GetStopA() == prev || c.GetStopB() == prev));
      }
    }

    return null;
  }

  private float CalculateRandomizedTime(float mean, float stdDeviation)
  {
    double u1 = 1.0 - Random.NextDouble();
    double u2 = 1.0 - Random.NextDouble();
    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

    float noisyTime = mean + stdDeviation * (float)randStdNormal;
    return Math.Max(0f, noisyTime);
  }

  public void OnError(Exception error)
  {
    Console.WriteLine($"BusTimer error: {error.Message}");
  }

  public void OnCompleted()
  {
    _unsubscriber?.Dispose();
  }

  public void Unsubscribe()
  {
    _unsubscriber?.Dispose();
  }
}