using Backend.Dynamique.Entities;
using Backend.Reseau.Elements;

namespace Backend.Dynamique.State;

public class Arreter : IEtatBus
{
  public void Handle(Bus bus)
  {
    ElementBusLine currentStop = bus.getOnline().GetStopByIndex(bus.getIndexPositionOnLine());
    Console.WriteLine($"Bus {bus.GetImmatriculation()} est arrêté à {currentStop.GetNameBusStop()}.");
  }
}