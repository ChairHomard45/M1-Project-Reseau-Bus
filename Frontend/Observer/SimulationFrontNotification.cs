<<<<<<<< HEAD:Backend/Observer/SimulationFrontNotification.cs
﻿using Backend.Dynamique.Entities;

namespace Frontend.Observer
========
﻿namespace Frontend.Observer
>>>>>>>> 27a3e27f462507c5f3f2e91b9a35cb64b3b70f6b:Frontend/Observer/SimulationFrontNotification.cs
{
    public class SimulationFrontNotification
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

        public SimulationFrontNotification(NotificationType type, string simulationName, TimeSpan? currentTime, TimeSpan? endTime, IReadOnlyList<Bus>? buses)
        {
            Type = type;
            SimulationName = simulationName;
            CurrentTime = currentTime;
            EndTime = endTime;
            Buses = buses;
        }
    }
}
