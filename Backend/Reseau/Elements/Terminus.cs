using System.Drawing;

namespace Backend.Reseau.Elements;

public class Terminus : ElementBusLine
{
  public Terminus(int idElement, string nameBusStop, Color elementColor) : base(idElement, nameBusStop, elementColor)
  {
  }

  private Connection? NextStop { get; set; }

  public override IEnumerable<Connection?> Connections => 
    NextStop == null ? Enumerable.Empty<Connection>() : new[] { NextStop };
  
  public void SetNextStop(Connection? stop)
  {
    NextStop = stop;
  }

  public override int CountConnections()
  {
    return NextStop == null ? 0 : 1;
  }
  
  public override bool IsTerminus()
  {
    return true;
  }
}