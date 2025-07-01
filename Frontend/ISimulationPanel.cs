using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    interface ISimulationPanel : IObserver<SimulationNotification>
    {
        void Show();
        void Hide();
    }
}
