using Backend.Reseau.Elements;

namespace Backend.Reseau;

public class BusNetwork
{
  private static BusNetwork? _instance;
  private static readonly object Lock = new();

  // Liste des arrêts du réseau
  private List<ElementBusLine> Stops { get; set; } = new();

  // Liste des lignes du réseau
  private List<BusLine> Lines { get; set; } = new();

  private BusNetwork()
  {
  }

  public static BusNetwork Instance
  {
    get
    {
      lock (Lock)
      {
        return _instance ??= new BusNetwork();
      }
    }
  }

  public void AddStop(ElementBusLine stop)
  {
    if (!Stops.Any(s => s.GetId() == stop.GetId()))
    {
      Stops.Add(stop);
    }
  }

  public int GetStopsCount()
  {
    return Stops.Count;
  }
  
  public int GetLineCount()
  {
    return Lines.Count;
  }
  
  public int GetLineStopsCount(string nomLine)
  {
    BusLine? line = Lines.FirstOrDefault(l => l.NomLine == nomLine);
    if (line == null)
      return 0;
    return line.CountStops();
  }

  // Ajouter une ligne, si pas déjà présente
  public void AddLine(BusLine line)
  {
    if (!Lines.Any(l => l.NomLine == line.NomLine))
    {
      Lines.Add(line);
    }
  }

  // Récupérer une ligne par nom
  public BusLine? GetLineByName(string nomLine)
  {
    return Lines.FirstOrDefault(l => l.NomLine == nomLine);
  }
  
  // Récupérer une ligne par nom
  public BusLine? GetLineByIndex(int index)
  {
    if (index < 0 || index >= Lines.Count)
      return null;
    return Lines.ElementAt(index);
  }

  // Récupérer un arrêt par Id
  public ElementBusLine? GetStopById(int id)
  {
    return Stops.FirstOrDefault(s => s.GetId() == id);
  }
  
// Cloner une ligne par nom
  public BusLine? CloneLineByName(string nomLine)
  {
    BusLine? originalLine = Lines?.FirstOrDefault(l => l.NomLine == nomLine);
    return originalLine != null ? (BusLine)originalLine.Clone() : null;
  }

  public List<BusLine> GetClonedLines()
  {
    return Lines.Select(l => (BusLine) l.Clone()).ToList();
  }

}