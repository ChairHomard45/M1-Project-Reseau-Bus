using Backend.Dynamique;
using Backend.Dynamique.Entities;
using Backend.Reseau;

namespace Backend.Utils.Loaders
{
  public class BusCreationLoader
  {
    /// <summary>
    /// Create buses for a given line based on its scheduled departure times.
    /// </summary>
    /// <param name="line">BusLine to create buses for</param>
    /// <param name="direction">Direction of buses (true/false)</param>
    /// <param name="numberOfBuses"></param>
    /// <param name="prefix">Optional bus ID prefix</param>
    /// <returns>List of created Bus instances</returns>
    public List<Bus> CreateBusesFromLineSchedule(BusLine line, bool direction, int numberOfBuses, string prefix = "BUS")
    {
      var buses = new List<Bus>();

      if (line.CountScheduledDepartures() == 0 || numberOfBuses <= 0)
        return buses;

      int scheduleCount = line.CountScheduledDepartures();

      for (int i = 0; i < numberOfBuses; i++)
      {
        int scheduleIndex = (int)Math.Floor(i * (double)scheduleCount / numberOfBuses);
        var departure = line.GetScheduledDepartureByIndex(scheduleIndex);

        string busId = $"{prefix}-{line.NomLine}-{departure:hhmm}";
        var bus = new Bus(busId, line, direction);

        buses.Add(bus);
      }

      return buses;
    }


    /// <summary>
    /// Create and add BusTimers for each created bus to a simulation.
    /// </summary>
    public void AddBusesToSimulation(Simulation simulation, List<Bus> buses)
    {
      foreach (var bus in buses)
      {
        simulation.AddBus(bus);
      }
    }
  }
}