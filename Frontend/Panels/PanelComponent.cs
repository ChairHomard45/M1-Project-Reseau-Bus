using Frontend.Observer;

namespace Frontend.Panels
{
  public abstract class PanelComponent : Panel, ISimulationPanel
  {
    public virtual void OnCompleted()
    {
    }

    public virtual void OnError(Exception error)
    {
      Console.WriteLine("Erreur : " + error.Message);
    }

    public abstract void OnNext(SimulationNotification value);

    public void ShowPanel() => this.Visible = true;
    public void HidePanel() => this.Visible = false;
  }
}