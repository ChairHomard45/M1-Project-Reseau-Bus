using Backend.Dynamique.Entities;
using Backend.Reseau.Elements;

namespace Backend.Dynamique.State;

public class EnCirculation : IEtatBus
{
  public void Handle(Bus bus)
  {
    var line = bus.getOnline();
    int currentIndex = bus.getIndexPositionOnLine();
    bool direction = bus.GetDirection();

    ElementBusLine previousStop = line.GetStopByIndex(currentIndex);
    ElementBusLine? nextStop = null;

    // Determine next stop based on direction
    if (direction)
    {
      if (currentIndex < line.CountStops() - 1)
        nextStop = line.GetStopByIndex(currentIndex + 1);
    }
    else
    {
      if (currentIndex > 0)
        nextStop = line.GetStopByIndex(currentIndex - 1);
    }

    if (nextStop != null)
    {
      Console.WriteLine($"Bus {bus.GetImmatriculation()} est en circulation entre les arrêts {previousStop.GetNameBusStop()} et {nextStop.GetNameBusStop()}");
    }
    else
    {
      Console.WriteLine($"Bus {bus.GetImmatriculation()} est en circulation, mais il n'y a pas d'arrêt suivant (fin de la ligne).");
    }
  }
}