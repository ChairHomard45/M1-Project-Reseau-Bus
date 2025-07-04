using System;
using Frontend.Observer;

namespace Frontend.Panels
{
  public partial class HeaderSimulationPanel : PanelComponent
  {
    // Example label controls assumed, or you can add your own UI controls
    private Label labelCurrentTime;
    private Label labelEndTime;
    private Label labelSimulationName;

    public HeaderSimulationPanel()
    {
      // Initialize labels or other UI controls here
      labelCurrentTime = new Label() { Location = new System.Drawing.Point(10, 10) };
      labelEndTime = new Label() { Location = new System.Drawing.Point(10, 30) };
      labelSimulationName = new Label() { Location = new System.Drawing.Point(10, 50) };
      Controls.Add(labelCurrentTime);
      Controls.Add(labelEndTime);
      Controls.Add(labelSimulationName);
    }

    public override void OnNext(SimulationFrontNotification frontNotification)
    {
      if (frontNotification.Type == SimulationFrontNotification.NotificationType.CurrentTimeUpdate)
      {
        labelCurrentTime.Text = $"Current Time: {frontNotification.CurrentTime?.ToString() ?? "--"}";
        labelEndTime.Text = $"End Time: {frontNotification.EndTime?.ToString() ?? "--"}";
        labelSimulationName.Text = $"Simulation: {frontNotification.SimulationName}";
      }
    }
  }
}