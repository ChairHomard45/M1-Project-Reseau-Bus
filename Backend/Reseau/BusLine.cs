using Backend.Reseau.Elements;
using Backend.Utils;

namespace Backend.Reseau;

public class BusLine : ICloneable
{
  // Nom de la ligne de bus: public get, private set
  public string NomLine { get; private set; }

  // Liste des arrêts (éléments) constituant la ligne: public get, private set
  private List<ElementBusLine> Stops { get; set; } = new();

  // Plage horaire de fonctionnement de la ligne (ex: 6h-22h): public get, private set
  private TimeRange OperationTimeRange { get; set; }

  // Horaires prévus de départ des bus sur cette ligne: public get, private set
  private List<TimeSpan> ScheduledDepartures { get; set; } = new();
  

  // Constructeur initialisant le nom et la plage horaire de fonctionnement
  public BusLine(string nomLine, TimeRange operationTimeRange)
  {
    NomLine = nomLine;
    OperationTimeRange = operationTimeRange;
  }

  // Méthode de clonage profond permettant de dupliquer la ligne et ses données associées
  public object Clone()
  {
    var cloned = (BusLine)this.MemberwiseClone();
    
    var stopMap = new Dictionary<ElementBusLine, ElementBusLine>();
    var clonedStops = this.Stops
      .Select(e => {
        var clone = (ElementBusLine)e.Clone();
        stopMap[e] = clone;
        return clone;
      })
      .ToList();

    foreach (var original in this.Stops)
    {
      var clonedStop = stopMap[original];
      var clonedConnections = original.Connections?
        .Select(c => c == null ? null : c.CloneWithMapping(stopMap))
        .ToList();

      switch (clonedStop)
      {
        case BusStop busStop:
          busStop.SetConnections(clonedConnections!);
          break;
        case Terminus terminus:
          terminus.SetNextStop(clonedConnections!.FirstOrDefault());
          break;
      }
    }

    cloned.Stops = clonedStops;
    cloned.ScheduledDepartures = new List<TimeSpan>(this.ScheduledDepartures);
    cloned.OperationTimeRange = this.OperationTimeRange;

    return cloned;
  }

  public int CountStops()
  {
    return this.Stops.Count;
  }
  
  public int CountScheduledDepartures()
  {
    return this.ScheduledDepartures.Count;
  }

  public TimeSpan GetScheduledDepartureByIndex(int index)
  {
    return this.ScheduledDepartures[index];
  }

  public void AddToScheduleDeparture(TimeSpan departure)
  {
    ScheduledDepartures.Add(departure);
  }

  public void AddToStops(ElementBusLine stop)
  {
    Stops.Add(stop);
  }

  public ElementBusLine GetStopByIndex(int index)
  {
    return Stops[index];
  }

  public List<ElementBusLine> GetStops()
  {
    return Stops;
  }

  public void SetScheduledDepartures(List<TimeSpan> scheduledDepartures)
  {
    ScheduledDepartures = scheduledDepartures;
  }
  
}