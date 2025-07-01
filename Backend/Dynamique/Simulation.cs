using Backend.Dynamique.ObserverObservablePattern;
using Backend.Statique;

namespace Backend.Dynamique;

public class Simulation : IObserver<NotificationChrono>
{
  private readonly ReseauBus _reseauBus;
  private readonly List<Bus> _busEnCirculation;
  private readonly Chrono _chronoSimu;
  private readonly TimeSpan _debutSimulation;
  private readonly TimeSpan _finSimulation;
  private readonly string _nomSimulation;

  private readonly Dictionary<NotificationChronoType, Action<NotificationChrono>> _handlers;
  
  public Simulation(string nomSimulation, TimeSpan debutSimulation, TimeSpan finSimulation)
  {
    _nomSimulation = nomSimulation;
    _debutSimulation = debutSimulation;
    _finSimulation = finSimulation;

    _reseauBus = ReseauBus.Instance;

    _chronoSimu = new Chrono();
    _chronoSimu.Subscribe(this);
    this.Subscribe(_chronoSimu);

    _busEnCirculation = new List<Bus>();

    InitializeBus();

    _handlers = new Dictionary<NotificationChronoType, Action<NotificationChrono>>
    {
      { NotificationChronoType.StartChrono, HandleStartChrono },
      { NotificationChronoType.StopChrono, HandleStopChrono },
      { NotificationChronoType.DureeChrono, HandleDureeChrono }
    };
  }


// ******************************************************************************************************
// ***                                                                                                ***
// ***                                 Implementation Observable for Chrono                           ***
// ***                                                                                                ***
// ******************************************************************************************************

  private readonly HashSet<NotificationChrono> _notificationsChrono = new();
  private readonly HashSet<IObserver<NotificationChrono>> _observateursChrono = new();

  public virtual void Subscribe(Chrono provider)
  {
    _cancellation = provider.Subscribe(this);
  }

  public virtual void Unsubscribe()
  {
    _cancellation?.Dispose();
  }

  public IDisposable Subscribe(IObserver<NotificationChrono> observateur)
  {
    // Si l'obervateur n'est pas encore dans la liste, on l'ajoute
    if (_observateursChrono.Add(observateur))
      // On lui envoie les notifications
      foreach (var notification in _notificationsChrono)
        observateur.OnNext(notification);

    return new Unsubscriber<NotificationChrono>(_observateursChrono, observateur);
  }

  private NotificationChrono CreateNotificationChrono(NotificationChronoType type, String? typeChrono, TimeSpan? dureeChrono)
  {
    return new NotificationChrono(type, typeChrono, dureeChrono);
  }

  private void NotifyChrono(NotificationChrono value)
  {
    _notificationsChrono.Add(value);
    foreach (IObserver<NotificationChrono> observer in _observateursChrono) observer.OnNext(value);
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                 Implementation Observer for Chrono                             ***
// ***                                                                                                ***
// ******************************************************************************************************

  private IDisposable? _cancellation;

  public void OnCompleted()
  {
    Console.WriteLine("Simulation : " + _nomSimulation + " terminer.");
  }

  public void OnError(Exception error)
  {
    Console.WriteLine("Simulation : " + _nomSimulation + " a rencontrer l'erreur suivante - " + error.Message);
  }

  public void OnNext(NotificationChrono value)
  {
    if (_handlers.TryGetValue(value.GetTypeNotification(), out var handler))
    {
      handler(value);
    }
    else
    {
      Console.WriteLine("Unknown notification type.");
    }

  }

  private void HandleStartChrono(NotificationChrono notification)
  {
    Console.WriteLine("Start Chrono: " + (notification.GetDureeRestante()?.ToString() ?? "No duration"));
  }

  private void HandleStopChrono(NotificationChrono notification)
  {
    Console.WriteLine("Stop Chrono: " + (notification.GetDureeRestante()?.ToString() ?? "No duration"));
  }
  
  private void HandleDureeChrono(NotificationChrono notification)
  {
    Console.WriteLine("Duree Chrono: " + (notification.GetDureeRestante()?.ToString() ?? "No duration"));
  }
  
// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Getter & Setter                                           ***
// ***                                                                                                ***
// ******************************************************************************************************

  public TimeSpan GetDebutTime()
  {
    return _debutSimulation;
  }

  public TimeSpan GetFinTime()
  {
    return _finSimulation;
  }

  public Chrono GetChrono()
  {
    return _chronoSimu;
  }
  
  public IReadOnlyList<Bus> GetBusEnCirculation() => _busEnCirculation.AsReadOnly();

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Private Functions                                         ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void InitializeBus()
  {
    for (var i = 0; i < 2; i++)
    {
      /* Initialisation des données pour chaque bus */
      var randomLignes = _reseauBus.GetRandomLigne();

      var randomSens = new Random().Next(2) == 0 ? 1 : -1;
      
      if (!randomLignes.GetElementLigne().Any()) return;
      
      var deArret = randomSens == 1
        ? randomLignes.GetElementLigne().First()
        : randomLignes.GetElementLigne().Last();
      var versArret = randomSens == 1
        ? randomLignes.GetElementLigne().Last()
        : randomLignes.GetElementLigne().First();
      var newBus = new Bus(
        "0" + i,
        randomLignes,
        randomSens,
        deArret,
        versArret,
        randomLignes.GetElementLigneRandom()
      );

      _busEnCirculation.Add(newBus);
    }
  }
}