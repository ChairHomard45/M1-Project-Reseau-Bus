using System.Text.Json;
using Backend.Utils.Dto.Stats;

namespace Backend.Utils.Factory;

public static class TimeStatsFactory
{
  public static TimeRange CreateFromStrings(string start, string end) =>
    new TimeRange(TimeSpan.Parse(start), TimeSpan.Parse(end));
  
  public static Dictionary<TimeRange, TravelStats> CreateDefaultTravelStats()
  {
    var timeRanges = GetDefaultTimeRanges();

    var travelStats = new Dictionary<TimeRange, TravelStats>();

    foreach (var range in timeRanges)
    {
      if (IsPeakMorning(range))
        travelStats[range] = new TravelStats(MeanTime: 8.0f, StdDeviation: 1.2f);
      else if (IsPeakEvening(range))
        travelStats[range] = new TravelStats(MeanTime: 7.5f, StdDeviation: 1.0f);
      else
        travelStats[range] = new TravelStats(MeanTime: 5.0f, StdDeviation: 0.5f);
    }

    return travelStats;
  }

  public static Dictionary<TimeRange, ServiceStats> CreateDefaultServiceStats()
  {
    var timeRanges = GetDefaultTimeRanges();

    var serviceStats = new Dictionary<TimeRange, ServiceStats>();

    foreach (var range in timeRanges)
    {
      if (IsPeakMorning(range))
        serviceStats[range] = new ServiceStats(MeanTime: 0.8f, StdDeviation: 0.2f);
      else if (IsPeakEvening(range))
        serviceStats[range] = new ServiceStats(MeanTime: 0.7f, StdDeviation: 0.15f);
      else
        serviceStats[range] = new ServiceStats(MeanTime: 0.5f, StdDeviation: 0.1f);
    }

    return serviceStats;
  }

  private static List<TimeRange> GetDefaultTimeRanges()
  {
    return new List<TimeRange>
    {
      CreateFromStrings("06:00", "07:00"),
      CreateFromStrings("07:00", "08:00"),
      CreateFromStrings("08:00", "09:00"),
      CreateFromStrings("09:00", "17:00"),
      CreateFromStrings("17:00", "19:00"),
      CreateFromStrings("19:00", "22:00")
    };
  }

  private static bool IsPeakMorning(TimeRange range)
  {
    return range.Start >= TimeSpan.FromHours(8) && range.End <= TimeSpan.FromHours(9);
  }

  private static bool IsPeakEvening(TimeRange range)
  {
    return range.Start >= TimeSpan.FromHours(17) && range.End <= TimeSpan.FromHours(19);
  }

  public static (Dictionary<TimeRange, TravelStats> travelStats, Dictionary<TimeRange, ServiceStats> serviceStats)
    LoadFromJson(string json)
  {
    var container = JsonSerializer.Deserialize<TimeRangeStatsContainer>(json);

    var travelStats = new Dictionary<TimeRange, TravelStats>();
    var serviceStats = new Dictionary<TimeRange, ServiceStats>();

    if (container == null)
      return (travelStats, serviceStats);

    foreach (var item in container.TimeRanges)
    {
      var range = CreateFromStrings(item.Start, item.End);

      travelStats[range] = new TravelStats(item.TravelStats.MeanTime, item.TravelStats.StdDeviation);

      serviceStats[range] = new ServiceStats(item.ServiceStats.MeanLoadingTime, item.ServiceStats.StdDeviation);
    }

    return (travelStats, serviceStats);
  }
}
