namespace Backend.Dynamique.Time;

public class TimeManager
{
  private readonly Dictionary<string, (double meanTime, double stdDev)> _travelTimes;
  private readonly Dictionary<string, (double meanTime, double stdDev)> _loadingTimes;

  public TimeManager()
  {
    _travelTimes = new Dictionary<string, (double, double)>();
    _loadingTimes = new Dictionary<string, (double, double)>();
  }

  public void AjouterTempsTrajet(string creneau, double meanTime, double stdDev)
  {
    _travelTimes[creneau] = (meanTime, stdDev);
  }

  public void AjouterTempsChargement(string creneau, double meanTime, double stdDev)
  {
    _loadingTimes[creneau] = (meanTime, stdDev);
  }

  public double GetRandomTravelTime(string creneau)
  {
    if (!_travelTimes.ContainsKey(creneau))
      throw new KeyNotFoundException($"Créneau {creneau} non trouvé.");

    var (mean, stdDev) = _travelTimes[creneau];
    var random = new Random();
    double time = mean + stdDev * (random.NextDouble() * 2 - 1); // Simple random variation
    return Math.Max(time, 0); // Ensure time is not negative
  }

  public double GetRandomLoadingTime(string creneau)
  {
    if (!_loadingTimes.ContainsKey(creneau))
      throw new KeyNotFoundException($"Créneau {creneau} non trouvé.");

    var (mean, stdDev) = _loadingTimes[creneau];
    var random = new Random();
    double time = mean + stdDev * (random.NextDouble() * 2 - 1);
    return Math.Max(time, 0);
  }
}
