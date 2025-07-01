using System.Timers;
using Timer = System.Timers.Timer;

namespace Backend.Dynamique.StateBus;

public class EnCirculation : IEtatBus
{
  private readonly Bus _bus;
  private Timer _timer;

  public EnCirculation(Bus bus)
  {
    _bus = bus;
    _timer = new Timer(GetRandomTravelTime().TotalMilliseconds);
    _timer.Elapsed += OnTimerElapsed;
    StartTimer();
  }

  private TimeSpan GetRandomTravelTime()
  {
    return TimeSpan.MaxValue;
  }
  public void OnChronoEnd(Bus bus)
  {
    bus.ChangeElements();
    bus.HandleStateChange(new Arreter(bus));
  }
  
  
// ******************************************************************************************************
// ***                                                                                                ***
// ***                                 Implementation Chrono                                          ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void StartTimer()
  {
    var previousId = _bus.GetPreviousArret()?.GetId().ToString() ?? "N/A";
    var currentId = _bus.GetSurArret()?.GetId().ToString() ?? "N/A";
    var nextId = _bus.GetNextArret()?.GetId().ToString() ?? "N/A";

    Console.WriteLine("Bus : " + _bus.GetImmatriculation() + " / De Arret: " + previousId + " / Arrive vers Arret: " + currentId + " / Arret suivant: " + nextId);
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
      Console.WriteLine(
        "    BUS - " + _bus.GetImmatriculation()
                     + " / Anciennement en Circulation de : "
                     + (_bus.GetPreviousArret()?.GetId().ToString() ?? "N/A")
                     + " / Vers Arret: "
                     + (_bus.GetNextArret()?.GetId().ToString() ?? "N/A")
      );
      Console.WriteLine(" Chrono EnCirculation has ended.");
      Console.WriteLine("Starting the changing of State to Arreter.");
      Console.WriteLine("------------------------------");
      _bus.ChronoEnd();
    }
  }
}