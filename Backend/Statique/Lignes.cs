namespace Backend.Statique;

public class Lignes : ICloneable
{
  protected List<ElementLigne> ElementDeLaLigne;
  protected string NomLigne;

  private TimeSpan _debutFonctionnement;
  private TimeSpan _finFonctionnement;

  public Lignes(string nomLigne)
  {
    NomLigne = nomLigne;
    ElementDeLaLigne = new List<ElementLigne>();
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Implementation Clone                                      ***
// ***                                                                                                ***
// ******************************************************************************************************
  public object Clone()
  {
    Lignes lignes = (Lignes)MemberwiseClone();
    lignes.ElementDeLaLigne = new List<ElementLigne>();

    Dictionary<int, ElementLigne> clonedElements = new Dictionary<int, ElementLigne>();

    foreach (var element in ElementDeLaLigne)
    {
      lignes.AjouterLigne((ElementLigne)element.Clone(clonedElements, this));
    }

    return lignes;
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                               Getter & Setter & public function                                ***
// ***                                                                                                ***
// ******************************************************************************************************

  public bool CheckElementLigne(ElementLigne element)
  {
    return ElementDeLaLigne.Contains(element);
  }

  public List<ElementLigne> GetElementLigne()
  {
    return new List<ElementLigne>(ElementDeLaLigne);
  }

  public string GetNom()
  {
    return NomLigne;
  }

  public ElementLigne GetElementLigneRandom()
  {
    return ElementDeLaLigne[new Random().Next(0, ElementDeLaLigne.Count)];
  }

  public void AjouterLigne(ElementLigne elementLigne)
  {
    if (ElementDeLaLigne.Contains(elementLigne)) return;

    if (elementLigne is Terminus && elementLigne.GetElementLignes().Count > 0)
    {
      return;
    }

    if (ElementDeLaLigne.Count > 0)
    {
      elementLigne.AjouterElementLigne(ElementDeLaLigne.Last());
      ElementDeLaLigne.Last().AjouterElementLigne(elementLigne);
    }

    ElementDeLaLigne.Add(elementLigne);
  }

  public void SetHoraire(TimeSpan debut, TimeSpan fin)
  {
    _debutFonctionnement = debut;
    _finFonctionnement = fin;
  }

  public (TimeSpan, TimeSpan) GetHoraire()
  {
    return (_debutFonctionnement, _finFonctionnement);
  }
}