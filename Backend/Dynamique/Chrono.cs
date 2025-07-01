using System.Diagnostics;
using System.Timers;
using Backend.Dynamique.ObserverObservablePattern;
using Timer = System.Timers.Timer;

namespace Backend.Dynamique;

public class Chrono : IObservable<NotificationChrono>
{
  // System.Timers contient un Timer spécifique aux WinForms
  private Timer? _chrono;

  // Heure de début du chnronomètre
  private DateTime _debut = DateTime.Now;

  // Durée du chronomètre en h, mn , sec
  private TimeSpan _dureeChrono;

  // Fréquence avec laquelle le Timer envoie le temps écoulé (en ms)
  private int _frequence = 1000;

  // Indique si le chrono est écoulé
  private bool _isActif = true;

  // Temps restant avant la fin du chrono
  private TimeSpan _tempsRestant;
  
  private readonly Dictionary<NotificationChronoType, Action<NotificationChrono>> _handlers;

  public Chrono()
  {
    _handlers = new Dictionary<NotificationChronoType, Action<NotificationChrono>>
    {
      { NotificationChronoType.StartChrono, HandleStartChrono },
      { NotificationChronoType.StopChrono, HandleStopChrono },
      { NotificationChronoType.DureeChrono, HandleDureeChrono }
    };
  }

  public void Start(int delai, String type, TimeSpan dureeChrono)
  {
    _isActif = true;
    _dureeChrono = dureeChrono;
    _tempsRestant = _dureeChrono;
    _frequence = delai;
    _chrono = new Timer(_frequence);
    if (type.Equals("Fast"))
      _chrono.Elapsed += TickMinute;
    if (type.Equals("Normal"))
      _chrono.Elapsed += Tick;
    _debut = DateTime.Now;
    _chrono.Enabled = true;
    Trace.WriteLine($"Start Chrono : {_debut} - type : {type}");
  }

  public void Stop()
  {
    if (_chrono != null)
    {
      _chrono.Enabled = false;
      _chrono.Stop();
    }
  }
  
// ******************************************************************************************************
// ***                                                                                                ***
// ***                           Implementation Observable for Simulation                             ***
// ***                                                                                                ***
// ******************************************************************************************************
  private readonly HashSet<IObserver<NotificationChrono>> _observateursChrono = new();
  private readonly HashSet<NotificationChrono> _notificationsChrono = new();
  
  public IDisposable Subscribe(IObserver<NotificationChrono> observer)
  {
    // Si l'obervateur n'est pas encore dans la liste, on l'ajoute
    if (_observateursChrono.Add(observer))
      // On lui envoie les notifications
      foreach (var notification in _notificationsChrono)
        observer.OnNext(notification);

    return new Unsubscriber<NotificationChrono>(_observateursChrono, observer);
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                             Implementation Observer for Simulation                             ***
// ***                                                                                                ***
// ******************************************************************************************************
  
  private IDisposable? _cancellation;

  public void OnCompleted()
  {
    Console.WriteLine("Chrono terminer.");
  }

  public void OnError(Exception error)
  {
    Console.WriteLine("Chrono a rencontrer l'erreur suivante - " + error.Message);
  }

  public void OnNext(NotificationChrono value)
  {
    if (_handlers.ContainsKey(value.GetTypeNotification()))
    {
      _handlers[value.GetTypeNotification()](value);
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
// ***                                      Private Functions                                         ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void Tick(object? sender, ElapsedEventArgs e)
  {
    if (_chrono != null && _tempsRestant <= TimeSpan.Zero)
    {
      _chrono.Stop();
      _chrono.Enabled = false;
      _isActif = false;
      Trace.WriteLine("Chrono arrêté");
    }
    else
    {
      _tempsRestant = _dureeChrono - (e.SignalTime - _debut);
    }

    var tempsRestantString = string.Format("{0:00}:{1:00}:{2:00}", _tempsRestant.Hours, _tempsRestant.Minutes,
      _tempsRestant.Seconds);
    Trace.WriteLine(tempsRestantString);
  }

  private void TickMinute(object? sender, ElapsedEventArgs e)
  {
    if (_chrono != null && _tempsRestant <= TimeSpan.Zero)
    {
      _chrono.Stop();
      _chrono.Enabled = false;
      _isActif = false;
      Trace.WriteLine("Chrono arrêté");
    }
    else
    {
      _tempsRestant = _tempsRestant.Subtract(TimeSpan.FromMinutes(1));
    }

    var tempsRestantString = string.Format("{0:00}:{1:00}:{2:00}", _tempsRestant.Hours, _tempsRestant.Minutes,
      _tempsRestant.Seconds);
    Trace.WriteLine(tempsRestantString);
  }
}