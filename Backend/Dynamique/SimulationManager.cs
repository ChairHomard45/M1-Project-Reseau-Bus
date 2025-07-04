using Backend.Dynamique.Clock;

namespace Backend.Dynamique;

public class SimulationManager
{
  private static SimulationManager? _instance;
  private static readonly object Lock = new();

  private readonly Dictionary<string, Simulation> _simulations = new();

  private SimulationManager() { }
  
  public static SimulationManager Instance
  {
    get
    {
      lock (Lock)
      {
        return _instance ??= new SimulationManager();
      }
    }
  }

  public void RegisterSimulation(Simulation simulation)
  {
    if (_simulations.ContainsKey(simulation.Name))
      throw new InvalidOperationException($"Simulation '{simulation.Name}' already exists.");

    _simulations[simulation.Name] = simulation;
  }

  public Simulation? GetSimulation(string name) =>
    _simulations.TryGetValue(name, out var sim) ? sim : null;

  public IEnumerable<Simulation> GetAllSimulations() => _simulations.Values;

  public void StartAll() => _simulations.Values.ToList().ForEach(sim => sim.Start());

  public void StopAll() => _simulations.Values.ToList().ForEach(sim => sim.Stop());

  public void ChangeStrategyForAll(ITimeUnitStrategy strategy)
  {
    foreach (var sim in _simulations.Values)
    {
      sim.ChangeTimeUnit(strategy);
    }
  }

  public void UnregisterSimulation(string name)
  {
    _simulations.Remove(name);
  }
}
