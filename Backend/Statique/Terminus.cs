namespace Backend.Statique;

public class Terminus : ElementLigne
{
  public Terminus(int id, string nom, float x, float y) : base(id, nom, x, y)
  {
  }

  public override bool IsTerminus()
  {
    return true;
  }

  public override void AjouterElementLigne(ElementLigne element)
  {
    if (Connections.Count > 0)
    {
      throw new InvalidOperationException($"{Id} - Terminus ne peut avoir que un Element lier.");
    }
    Connections.Add(element);
  }
}