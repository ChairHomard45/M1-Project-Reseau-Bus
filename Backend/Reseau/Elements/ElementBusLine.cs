using System.Drawing;
using Backend.Utils;

namespace Backend.Reseau.Elements;

public abstract class ElementBusLine : ICloneable
{
  private int Id { get; set; }
  private string NameBusStop { get; set; }
  public abstract IEnumerable<Connection?>? Connections { get;  }
  private Coordinate Position { get; set; }
  private Color ElementColor { get; set; }

  
  private Dictionary<TimeRange, ServiceStats> ServiceTimes { get; set; }

  public ElementBusLine(int idElement, string nameBusStop, Color elementColor)
  {
    Id = idElement;
    NameBusStop = nameBusStop;
    ServiceTimes = new Dictionary<TimeRange, ServiceStats>();
    ElementColor = elementColor;
  }

  public int GetId()
  {
    return Id;
  }

  public string GetNameBusStop()
  {
    return NameBusStop;
  }

  public float GetPositionX()
  {
    return Position.X;
  }
  
  public float GetPositionY()
  {
    return Position.Y;
  }

  public void SetPosition(Coordinate position)
  {
    Position = position;
  }

  public void SetServiceTimes(Dictionary<TimeRange, ServiceStats> serviceTimes)
  {
    ServiceTimes = serviceTimes;
  }

  public Dictionary<TimeRange, ServiceStats> GetServiceTimes()
  {
    return ServiceTimes;
  }

  public object Clone()
  {
    var clone = (ElementBusLine)this.MemberwiseClone();
    
    switch (clone)
    {
      case BusStop busStop:
        busStop.SetConnections(new List<Connection?>());
        break;
      case Terminus terminus:
        terminus.SetNextStop(null);
        break;
    }

    clone.ServiceTimes = this.ServiceTimes.ToDictionary(
      kvp => kvp.Key,
      kvp => kvp.Value with { }
    );
    clone.Position = this.Position;

    return clone;
  }

  public abstract int CountConnections();
  
  public abstract bool IsTerminus();
}