using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Panels
{
    public abstract class PanelComponent : Panel
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

        public void Show() => this.Visible = true;
        public void Hide() => this.Visible = false;
    }
}
