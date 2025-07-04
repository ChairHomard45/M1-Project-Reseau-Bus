using Backend.Dynamique;
using Backend.Dynamique.Clock;
using Backend.Reseau;
using Backend.Utils.Loaders;

namespace Backend;

class Program
{
  static void Main(string[] args)
  {
    string linesFilePath = "../../../assets/lignes.json";
    string stopsFilePath = "../../../assets/reseau.json";

    JsonFileLoader.LoadFromFiles(linesFilePath, stopsFilePath);

    //foreach (var line in BusNetwork.Instance.GetClonedLines())
    //{
    //  Console.WriteLine($"Ligne: {line.NomLine}");
    //  PrintStopsAndConnections(line);
    //}

    Console.WriteLine("\n--- Simulation Commencé ---");
    RunSimulation();
  }

  private static void PrintStopsAndConnections(BusLine line)
  {
    foreach (var stop in line.GetStops())
    {
      Console.WriteLine($"  Stop {stop.GetId()}: {stop.GetNameBusStop()} a ({stop.GetPositionX()}, {stop.GetPositionY()})");

      var connections = stop.Connections?.Where(c => c != null).ToList();

      if (connections != null && connections.Count > 0)
      {
        foreach (var conn in connections)
        {
          if (conn?.GetOtherStop(stop) == null)
          {
            Console.WriteLine("    ⚠ Warning: Stop n'est fait pas partie de la connexion!");
            continue;
          }

          var otherStop = conn.GetOtherStop(stop);
          Console.WriteLine($"    -> Connecté au stop {otherStop?.GetId()}: {otherStop?.GetNameBusStop()}");
        }
      }
      else
      {
        Console.WriteLine("    -> Pas de Connection");
      }
    }
  }


    private static void RunSimulation()
    {
        if (BusNetwork.Instance.GetLineCount() == 0)
        {
            Console.WriteLine("No lines loaded.");
            return;
        }

        var startTime = TimeSpan.FromHours(6);
        var endTime = TimeSpan.FromHours(20);
        var initialStrategy = new MinutesStrategy();
        var simulation = new Simulation("Simulation Multi-Lignes", startTime, endTime, initialStrategy, tickIntervalMs: 300);

        var loader = new BusCreationLoader();

        // Sélectionner une ligne pour la démonstration
        var ligneSelectionner = BusNetwork.Instance.GetLineByIndex(1);

        // Créer 2 bus dans la direction "avant"
        var busesForward = loader.CreateBusesFromLineSchedule(ligneSelectionner, direction: true, numberOfBuses: 1, prefix: "FORWARD");

        var busesBackward = loader.CreateBusesFromLineSchedule(ligneSelectionner, direction: false, numberOfBuses:1, prefix: "BACKWARD");

        loader.AddBusesToSimulation(simulation, busesForward);

        loader.AddBusesToSimulation(simulation, busesBackward);
      

        SimulationManager.Instance.RegisterSimulation(simulation);
        simulation.Start();

        while (simulation.CurrentTime < simulation.EndTime)
        {
            Console.WriteLine($"Time: {simulation.CurrentTime:hh\\:mm\\:ss}");
            Thread.Sleep(300);
        }

        Console.WriteLine("✅ Simulation complete.");
    }

}