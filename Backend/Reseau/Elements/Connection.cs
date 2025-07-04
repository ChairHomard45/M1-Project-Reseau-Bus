using Backend.Utils;

namespace Backend.Reseau.Elements;

public class Connection
{
  private ElementBusLine StopA { get; }
  private ElementBusLine StopB { get; }
  private Dictionary<TimeRange, TravelStats> TravelStats { get; set; }

  public Connection(ElementBusLine stopA, ElementBusLine stopB, Dictionary<TimeRange, TravelStats> travelStats)
  {
    TravelStats = travelStats;
    StopA = stopA;
    StopB = stopB;
  }

  public object Clone()
  {
    return new Connection(
      StopA,
      StopB,
      TravelStats.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value with { }
      )
    );
  }
  public Connection CloneWithMapping(Dictionary<ElementBusLine, ElementBusLine> stopMap)
  {
    return new Connection(
      stopMap[StopA],
      stopMap[StopB],
      TravelStats.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value with { }
      )
    );
  }


  public ElementBusLine? GetOtherStop(ElementBusLine stop)
  {
    if (stop == StopA) return StopB;
    if (stop == StopB) return StopA;
    return null;
  }

  public ElementBusLine GetStopA()
  {
    return StopA;
  }
  
  public ElementBusLine GetStopB()
  {
    return StopB;
  }
  
  private static readonly Random Random = new Random();

  public float CalculateRandomizedTravelTime(TimeSpan currentTime)
  {
    var timeRange = TravelStats.Keys.FirstOrDefault(tr => tr.Contains(currentTime));

    if (timeRange == null)
      throw new InvalidOperationException("No matching time range found for the given time.");

    var stats = TravelStats[timeRange];
    
    double u1 = 1.0 - Random.NextDouble();
    double u2 = 1.0 - Random.NextDouble();
    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

    float noisyBaseTime = stats.MeanTime + stats.StdDeviation * (float)randStdNormal;

    double distance = GetDistance(); 

    float scaledTime = noisyBaseTime * (float)distance;

    return Math.Max(0f, scaledTime);
  }
  
  public double GetDistance()
  {
    double dx = StopA.GetPositionX() - StopB.GetPositionX();
    double dy = StopA.GetPositionY() - StopB.GetPositionY();
    return Math.Sqrt(dx * dx + dy * dy);
  }

}