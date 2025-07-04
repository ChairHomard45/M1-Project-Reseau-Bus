using Backend.Dynamique.Observer;
using Backend.Dynamique.State;
using Backend.Reseau;
using Backend.Reseau.Elements;
using Backend.Utils;

namespace Backend.Dynamique.Entities;

public class Bus : IObserver<BusTimerNotification>, IObservable<BusTimerNotification>
{
  private IEtatBus _busState;
  private readonly string _immatriculation;
  private bool _direction; // 1 - Normal / 0 - Inverse
  private BusLine _onLine;
  private int _indexPositionOnLine;
  private ElementBusLine? _previousStop;
  
  private readonly List<IObserver<BusTimerNotification>> _observers = new();
  private readonly object _lock = new();

  public Bus(string immatriculation, BusLine line, bool direction = true)
  {
    _immatriculation = immatriculation;
    _onLine = line;
    _direction = direction;
    _indexPositionOnLine = direction ? 0 : line.CountStops() - 1;
    _busState = new Arreter();
  }

  public IEtatBus GetBusState()
  {
    return _busState;
  }

  public string GetImmatriculation()
  {
    return _immatriculation;
  }

  public bool GetDirection()
  {
    return _direction;
  }

  public BusLine getOnline()
  {
    return _onLine;
  }

  public int getIndexPositionOnLine()
  {
    return _indexPositionOnLine;
  }

  public void MoveNext()
  {
    var currentStop = _onLine.GetStopByIndex(_indexPositionOnLine);
    if (currentStop.IsTerminus() && _indexPositionOnLine == 0 && _direction == false)
    {
      _direction = true;
    }
    else if (currentStop.IsTerminus() && _indexPositionOnLine == _onLine.CountStops() - 1 && _direction == true)
    {
      _direction = false;
    }

    if (currentStop.Connections != null)
    {
      var possibleNextStops = currentStop.Connections
        .Select(c => c.GetOtherStop(currentStop))
        .Where(stop =>
          currentStop.IsTerminus() || stop != _previousStop
        )
        .ToList();

      var orderedStops = _direction
        ? _onLine.GetStops().Skip(_indexPositionOnLine + 1)
        : _onLine.GetStops().Take(_indexPositionOnLine).Reverse();

      var nextStop = orderedStops.FirstOrDefault(possibleNextStops.Contains);

      _previousStop = currentStop;
      if (nextStop != null) _indexPositionOnLine = _onLine.GetStops().IndexOf(nextStop);
    }
  }
  
  public void SetState(IEtatBus newState)
  {
    _busState = newState;
    _busState.Handle(this);
  }

  public void OnNext(BusTimerNotification notification)
  {
    if (notification.Bus != this)
      return;

    switch (notification.Type)
    {
      case BusTimerNotification.NotificationType.MoveNext:
        MoveNext();
        NotifyObservers(new BusTimerNotification(this, BusTimerNotification.NotificationType.MoveNext));
        break;

      case BusTimerNotification.NotificationType.ChangeStateToStop:
        SetState(new Arreter());
        NotifyObservers(new BusTimerNotification(this, BusTimerNotification.NotificationType.ChangeStateToStop));
        break;

      case BusTimerNotification.NotificationType.ChangeStateToCirculation:
        SetState(new EnCirculation());
        NotifyObservers(new BusTimerNotification(this, BusTimerNotification.NotificationType.ChangeStateToCirculation));
        break;
    }
  }
  private void NotifyObservers(BusTimerNotification notification)
  {
    List<IObserver<BusTimerNotification>> observersCopy;
    lock (_lock)
    {
      observersCopy = new List<IObserver<BusTimerNotification>>(_observers);
    }

    foreach (var obs in observersCopy)
    {
      obs.OnNext(notification);
    }
  }

  public void OnCompleted()
  {
    throw new NotImplementedException();
  }

  public void OnError(Exception error)
  {
    throw new NotImplementedException();
  }

  public IDisposable Subscribe(IObserver<BusTimerNotification> observer)
  {
    lock (_lock)
    {
      if (!_observers.Contains(observer))
        _observers.Add(observer);
    }
    return new Unsubscriber<BusTimerNotification>(_observers, observer);
  }
}