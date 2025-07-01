using Frontend.Observer;

namespace Frontend.Panels
{
    interface ISimulationPanel : IObserver<SimulationNotification>
    {
        void Show();
        void Hide();
    }
}
