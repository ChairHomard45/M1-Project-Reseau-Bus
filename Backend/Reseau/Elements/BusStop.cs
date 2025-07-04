using System.Drawing;

namespace Backend.Reseau.Elements;

public class BusStop : ElementBusLine
{
  private List<Connection?> _connections = new ();

  public BusStop(int idElement, string nameBusStop, Color elementColor) : base(idElement, nameBusStop, elementColor)
  {
  }

  public override IEnumerable<Connection?> Connections => _connections;

  public void SetConnections(List<Connection?> connections)
  {
    _connections = connections;
  }
  
  public void AddConnections(Connection? connections)
  {
    _connections.Add(connections);
  }

  public override int CountConnections()
  {
    return _connections.Count;
  }
  
  public override bool IsTerminus()
  {
    return false;
  }
}