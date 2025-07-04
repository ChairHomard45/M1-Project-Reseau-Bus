namespace Backend.Utils.Dto.Stats;

public class TimeRangeStatsDto
{
  public string Start { get; set; } = "";
  public string End { get; set; } = "";
  public TravelStatsDto TravelStats { get; set; } = new();
  public ServiceStatsDto ServiceStats { get; set; } = new();
}