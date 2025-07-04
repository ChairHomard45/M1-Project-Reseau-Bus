namespace Backend.Dynamique.Clock;

public class HoursStrategy : ITimeUnitStrategy
{
  public double ConvertToSeconds(double value) => value * 60 * 60;

  public TimeSpan AdvanceSimulationTime(TimeSpan currentTime) => currentTime.Add(TimeSpan.FromHours(1));
}