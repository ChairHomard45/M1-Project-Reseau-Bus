using System.Drawing;
using System.Text.Json;
using Backend.Reseau;
using Backend.Reseau.Elements;
using Backend.Utils.Dto.Json;
using Backend.Utils.Factory;

namespace Backend.Utils.Loaders;

public static class BusNetworkLoader
{
  public static void LoadFromJsonStrings(string linesJson, string stopsJson)
  {
    var lines = JsonSerializer.Deserialize<List<JsonLine>>(linesJson);
    var extraStops = JsonSerializer.Deserialize<List<JsonStop>>(stopsJson);

    if (lines != null)
    {
      foreach (var line in lines)
      {
        var start = TimeSpan.Parse("06:00");
        var end = TimeSpan.Parse("22:00");
        var busLine = new BusLine(line.nomLigne, new TimeRange(start, end));

        // Parse and assign scheduled departures
        if (line.ScheduledDepartures != null && line.ScheduledDepartures.Count > 0)
        {
          foreach (var departureStr in line.ScheduledDepartures)
          {
            if (TimeSpan.TryParse(departureStr, out var departureTime))
            {
              busLine.AddToScheduleDeparture(departureTime);
            }
            else
            {
              Console.WriteLine($"Warning: Invalid scheduled departure '{departureStr}' for line {line.nomLigne}");
            }
          }
        }
        else
        {
          busLine.SetScheduledDepartures(GenerateDefaultSchedule(start, end, intervalMinutes: 15));
        }

        // Add stops
        for (int i = 0; i < line.Arrets.Count; i++)
        {
          var stopJson = line.Arrets[i];
          ElementBusLine busStop;

          if (i == line.Arrets.Count - 1 || i == 0)
          {
            busStop = CreateTerminusFromJson(stopJson);
          }
          else
          {
            busStop = CreateStopFromJson(stopJson);
          }

          busLine.AddToStops(busStop);
          BusNetwork.Instance.AddStop(busStop);
        }

        // Create connections (same as your code)
        for (int i = 0; i < busLine.CountStops() - 1; i++)
        {
          var stopA = busLine.GetStopByIndex(i);
          var stopB = busLine.GetStopByIndex(i + 1);
          var travelStats = TimeStatsFactory.CreateDefaultTravelStats();

          var connection = new Connection(stopA, stopB, travelStats);

          if (stopA is BusStop busStopA)
          {
            busStopA.AddConnections(connection);
          }
          else if (stopA is Terminus terminusA)
          {
            terminusA.SetNextStop(connection);
          }

          if (stopB is BusStop busStopB)
          {
            busStopB.AddConnections(connection);
          }
          else if (stopB is Terminus terminusB)
          {
            terminusB.SetNextStop(connection);
          }
        }

        BusNetwork.Instance.AddLine(busLine);
      }
    }

    if (extraStops != null)
    {
      foreach (var stop in extraStops)
      {
        var busStop = CreateStopFromJson(stop);
        BusNetwork.Instance.AddStop(busStop);
      }
    }
  }

  private static List<TimeSpan> GenerateDefaultSchedule(TimeSpan start, TimeSpan end, int intervalMinutes)
  {
    var schedule = new List<TimeSpan>();
    for (var time = start; time <= end; time = time.Add(TimeSpan.FromMinutes(intervalMinutes)))
    {
      schedule.Add(time);
    }

    return schedule;
  }


  private static BusStop CreateStopFromJson(JsonStop jsonStop)
  {
    float x = float.Parse(jsonStop.X_coord.Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture);
    float y = float.Parse(jsonStop.Y_coord.Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture);
    var coordinate = new Coordinate(x, y);
    var busStop = new BusStop(jsonStop.id, jsonStop.nomArret, Color.LightGray);
    busStop.SetPosition(coordinate);

    // Initialize default ServiceTimes
    busStop.SetServiceTimes(TimeStatsFactory.CreateDefaultServiceStats());

    return busStop;
  }

  private static Terminus CreateTerminusFromJson(JsonStop jsonStop)
  {
    float x = float.Parse(jsonStop.X_coord.Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture);
    float y = float.Parse(jsonStop.Y_coord.Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture);
    var coordinate = new Coordinate(x, y);
    var terminus = new Terminus(jsonStop.id, jsonStop.nomArret, Color.Red);
    terminus.SetPosition(coordinate);

    terminus.SetServiceTimes(TimeStatsFactory.CreateDefaultServiceStats());

    return terminus;
  }
}