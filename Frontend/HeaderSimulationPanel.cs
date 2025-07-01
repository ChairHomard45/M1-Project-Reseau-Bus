using Frontend.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    class HeaderSimulationPanel : Panel, ISimulationPanel
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Erreur : ", error.Message);
        }


        public void OnNext(SimulationNotification value)
        {
            throw new NotImplementedException();
        }
    }
}
