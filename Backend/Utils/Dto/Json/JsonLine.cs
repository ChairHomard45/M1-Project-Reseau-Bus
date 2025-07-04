namespace Backend.Utils.Dto.Json;

public class JsonLine
{
  public string nomLigne { get; set; }
  public List<JsonStop> Arrets { get; set; }
  public List<string> ScheduledDepartures { get; set; } = new();
}