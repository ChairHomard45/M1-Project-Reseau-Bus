using System.Timers;
using Timer = System.Timers.Timer;

namespace Backend.Dynamique.StateBus;

public class Arreter : IEtatBus
{
  private readonly Bus _bus;
  private Timer _timer;
  private static readonly Random _random = new Random();

  public Arreter(Bus bus)
  {
    _bus = bus;
    _timer = new Timer(RandomArretTime().TotalMilliseconds);
    _timer.Elapsed += OnTimerElapsed; // subscribe once here
    StartTimer();
  }

  public void OnChronoEnd(Bus bus)
  {
    bus.ChangeElements();
    bus.HandleStateChange(new EnCirculation(bus));
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                 Implementation Chrono                                          ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void StartTimer()
  {
    var currentId = _bus.GetSurArret()?.GetId().ToString() ?? "N/A";
    Console.WriteLine("Bus : " + _bus.GetImmatriculation() + " / Sur Arret: " + currentId);
    _timer.Start();
    _timer.Enabled = true;
  }

  private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
  {
    if (_timer != null && _timer.Enabled)
    {
      _timer.Stop();
      _timer.Enabled = false;
      Console.WriteLine("------------------------------");
      Console.WriteLine("    BUS - " + _bus.GetImmatriculation() + " / Anciennement sur Arret: " + (_bus.GetSurArret()?.GetId().ToString() ?? "N/A"));
      Console.WriteLine(" Chrono Arreter has ended.");
      Console.WriteLine(" Starting the changing of State to EnCirculation.");
      Console.WriteLine("------------------------------");
      Console.Write("Ligne : ");
      foreach (var arret in _bus.GetLignes().GetElementLigne())
      {
        Console.Write(arret.GetId() + " "); // or arret.GetNom() if you have names
      }

      _bus.ChronoEnd();
    }
  }

  private TimeSpan RandomArretTime()
  {
    int randomSeconds = _random.Next(5, 10); // entre 5 et 9 sec
    return TimeSpan.FromSeconds(randomSeconds);
    //
    int randomMinutes = _random.Next(1, 3); 
    // Console.WriteLine("Random duree Arret : " + randomMinutes.ToString("F2") + " minutes");
    return TimeSpan.FromMinutes(randomMinutes);
  }
}