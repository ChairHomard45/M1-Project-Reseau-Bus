namespace Backend.Dynamique.Clock;

public class MinutesStrategy : ITimeUnitStrategy
{
  public double ConvertToSeconds(double value) => value * 60;

  public TimeSpan AdvanceSimulationTime(TimeSpan currentTime) => currentTime.Add(TimeSpan.FromMinutes(1));
}