namespace Backend.Dynamique.Clock;

public interface ITimeUnitStrategy
{
  double ConvertToSeconds(double value);
  TimeSpan AdvanceSimulationTime(TimeSpan currentTime);
}
