namespace Backend.Statique;

public class Arrets : ElementLigne
{
  public Arrets(int id, string nom, float x, float y) : base(id, nom, x, y)
  {
  }

  public override bool IsTerminus()
  {
    return false;
  }

  public override void AjouterElementLigne(ElementLigne element)
  {
    if (!Connections.Contains(element)) Connections.Add(element);
  }
}