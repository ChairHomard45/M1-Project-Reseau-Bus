using System.Drawing;

namespace Backend.Statique;

public abstract class ElementLigne : ICloneable
{
  protected Color ColorPoint;
  protected List<ElementLigne> Connections;
  protected int Id;
  protected string NomArret;
  protected float XCoordinate;
  protected float YCoordinate;

  public ElementLigne(int id, string nom, float x, float y)
  {
    Id = id;
    NomArret = nom;
    XCoordinate = x;
    YCoordinate = y;
    ColorPoint = Color.Black;
    Connections = new List<ElementLigne>();
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Implementation Clone                                      ***
// ***                                                                                                ***
// ******************************************************************************************************

  public object Clone()
  {
    var clone = (ElementLigne)MemberwiseClone();
    clone.Connections = new List<ElementLigne>();
    clone.ColorPoint = Color.FromName(ColorPoint.Name);
    
    Console.WriteLine("Test Cloning : " + Id  + " - is : " + GetType() + " - " + Connections.Count);
    
    foreach (var element in Connections)
    {
      clone.AjouterElementLigne((ElementLigne)element.Clone());
    }
    return clone;
  }
  
  public object Clone(Dictionary<int, ElementLigne> clonedElements, Lignes currentLigne)
  {
    // Check if the element has already been cloned to prevent infinite recursion
    if (clonedElements.ContainsKey(Id))
    {
      return clonedElements[Id];
    }

    var clone = (ElementLigne)MemberwiseClone();
    clone.Connections = new List<ElementLigne>();
    clone.ColorPoint = Color.FromName(ColorPoint.Name);

    clonedElements[Id] = clone; // Store this clone to prevent duplication in the future

    foreach (var element in Connections)
    {
      if (currentLigne != null && currentLigne.GetElementLigne().Any(e => e.Equals(element)))
      {
        if (element is Terminus && element.Connections.Count > 0)
        {
          continue;
        }
        
        clone.AjouterElementLigne((ElementLigne)element.Clone(clonedElements, currentLigne));
      }
    }

    return clone;
  }



// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Getter & Setter                                           ***
// ***                                                                                                ***
// ******************************************************************************************************

  public int GetId()
  {
    return Id;
  }

  public float GetXCoordinate()
  {
    return XCoordinate;
  }

  public float GetYCoordinate()
  {
    return YCoordinate;
  }

  public List<ElementLigne> GetElementLignes()
  {
    return new List<ElementLigne>(Connections);
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Abstract                                                  ***
// ***                                                                                                ***
// ******************************************************************************************************

  public abstract bool IsTerminus();

  public abstract void AjouterElementLigne(ElementLigne element);
}