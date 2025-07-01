using Backend.Util;
using Newtonsoft.Json.Linq;

namespace Backend.Statique;

public class ReseauBus
{
  private static ReseauBus? _instance;
  private static readonly object Lock = new();
  private readonly List<ElementLigne> _allElements;
  private readonly List<Lignes> _lignes;

  private ReseauBus()
  {
    _allElements = new List<ElementLigne>();
    _lignes = new List<Lignes>();
    ConstructReseau();
    //DisplayLignes();
  }

  public static ReseauBus Instance
  {
    get
    {
      lock (Lock)
      {
        return _instance ??= new ReseauBus();
      }
    }
  }


// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Getter & Setter                                           ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void DisplayLignes()
  {
    ConsoleColor[] colors =
    {
      ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Yellow,
      ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkMagenta,
      ConsoleColor.DarkYellow
    };
    var colorIndex = 0;
    foreach (var ligne in _lignes)
    {
      Console.ForegroundColor = colors[colorIndex % colors.Length];
      Console.WriteLine("Ligne : " + ligne.GetNom());
      ligne.GetElementLigne().ForEach(e =>
        {
          List<ElementLigne> ligneLignes = e.GetElementLignes();
          Console.Write("Arret Id : " + e.GetId() + " Lier aux arrets : [");
          ligneLignes.ForEach(ep =>
            {
              Console.Write(ep.GetId());
              if (ligneLignes.Count > 1) Console.Write(", ");
            }
          );
          Console.Write("] \n");
        }
      );
      Console.WriteLine();
      Console.ResetColor();
      colorIndex++;
    }

    Console.ResetColor();
  }

  public Lignes GetRandomLigne()
  {
    int randomLigne = new Random().Next(0, _lignes.Count);
    return (Lignes)_lignes[randomLigne].Clone();
  }

  public ElementLigne GetNextElementDeLigne(Lignes actualLigne, ElementLigne actualElement, int sens)
  {
    int indexElement = actualLigne.GetElementLigne().FindIndex(e => e.GetId() == actualElement.GetId());
    if (indexElement == -1)
      throw new InvalidOperationException("Element not found in Ligne.");

    if (sens == 1)
    {
      indexElement++;
      if (indexElement >= actualLigne.GetElementLigne().Count)
        indexElement = 0;

      return actualLigne.GetElementLigne()[indexElement];
    }

    if (sens == -1)
    {
      indexElement--;
      if (indexElement < 0)
        indexElement = actualLigne.GetElementLigne().Count - 1;
      return actualLigne.GetElementLigne()[indexElement];
    }

    throw new InvalidOperationException("Sens n'as pas une valeur correct.");
  }


// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Private Functions                                         ***
// ***                                                                                                ***
// ******************************************************************************************************

  private void ConstructReseau()
  {
    var path = "../../../assets/lignes.json";
    var jsonString = File.ReadAllText(path);
    var jsonArray = JArray.Parse(jsonString);

    Dictionary<int, ElementLigne> uniqueElements = new Dictionary<int, ElementLigne>();

    foreach (var ligne in jsonArray)
    {
      var lignes = new Lignes(ligne["nomLigne"]?.ToString() ?? string.Empty);
      if (ligne["Arrets"] == null) throw new NullReferenceException("Clef de arrets n'existe pas dans le Jtoken");

      var arretsArray = ligne["Arrets"]?.ToObject<JArray>() ?? new JArray();
      if (arretsArray == null || !arretsArray.Any()) throw new NullReferenceException("arretsArray est null ou vide.");

      var firstArret = arretsArray.First;
      var lastArret = arretsArray.Last;
      if (firstArret == null || lastArret == null) throw new NullReferenceException("First ou Last arret est null.");

      var firstId = JsonParseJtoken.ParseIntFromJtoken(firstArret, "id");
      var lastId = JsonParseJtoken.ParseIntFromJtoken(lastArret, "id");

      if (!uniqueElements.ContainsKey(firstId))
      {
        uniqueElements[firstId] = new Terminus(
          firstId,
          JsonParseJtoken.ParseStringFromJtoken(firstArret, "nomArret"),
          JsonParseJtoken.ParseFloatFromJtoken(firstArret, "X_coord"),
          JsonParseJtoken.ParseFloatFromJtoken(firstArret, "Y_coord")
        );
        AjouterElementLigne(uniqueElements[firstId]);
      }

      lignes.AjouterLigne(uniqueElements[firstId]);

      for (var i = 0; i < arretsArray.Count; i++)
      {
        var arretId = JsonParseJtoken.ParseIntFromJtoken(arretsArray[i], "id");

        if (!uniqueElements.ContainsKey(arretId))
        {
          uniqueElements[arretId] = new Arrets(
            arretId,
            JsonParseJtoken.ParseStringFromJtoken(arretsArray[i], "nomArret"),
            JsonParseJtoken.ParseFloatFromJtoken(arretsArray[i], "X_coord"),
            JsonParseJtoken.ParseFloatFromJtoken(arretsArray[i], "Y_coord")
          );
          AjouterElementLigne(uniqueElements[arretId]);
        }

        lignes.AjouterLigne(uniqueElements[arretId]);
      }

      if (!uniqueElements.ContainsKey(lastId))
      {
        uniqueElements[lastId] = new Terminus(
          lastId,
          JsonParseJtoken.ParseStringFromJtoken(lastArret, "nomArret"),
          JsonParseJtoken.ParseFloatFromJtoken(lastArret, "X_coord"),
          JsonParseJtoken.ParseFloatFromJtoken(lastArret, "Y_coord")
        );
        AjouterElementLigne(uniqueElements[lastId]);
      }

      lignes.AjouterLigne(uniqueElements[lastId]);

      AjouterLignes(lignes);
    }

    //Console.WriteLine("Count of lignes : " + _lignes.Count + " | ElementLignes : " + _allElements.Count +
    //                " Sans Duplication");
  }

  private void AjouterLignes(Lignes lignes)
  {
    if (!_lignes.Contains(lignes))
    {
      _lignes.Add(lignes);
      return;
    }

    throw new InvalidOperationException("Ligne existe déjà");
  }

  private void AjouterElementLigne(ElementLigne elementLigne)
  {
    if (_allElements.Any(ele => ele.GetId() == elementLigne.GetId()))
    {
      Console.WriteLine($"Duplicate found: {elementLigne.GetId()} - {elementLigne.GetId()}");
      return;
    }

    _allElements.Add(elementLigne);
  }
  
}