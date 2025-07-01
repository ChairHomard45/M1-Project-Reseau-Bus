using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Observer
{
    public class SimulationNotification
    {
        public enum NotificationType
        {
            Start,
            Stop,
            CurrentTimeUpdate,
            BusPositionsUpdate,
            SimulationInfoUpdate,

            Other
        }

        public NotificationType Type { get; }
        public string SimulationName { get; }
        public TimeSpan? CurrentTime { get; }
        public TimeSpan? EndTime { get; }

        public IReadOnlyList<Bus>? Buses { get; }

        public SimulationNotification(NotificationType type, string simulationName, TimeSpan? currentTime, TimeSpan? endTime, IReadOnlyList<Bus>? buses)
        {
            Type = type;
            SimulationName = simulationName;
            CurrentTime = currentTime;
            EndTime = endTime;
            Buses = buses;
        }
    }
}
