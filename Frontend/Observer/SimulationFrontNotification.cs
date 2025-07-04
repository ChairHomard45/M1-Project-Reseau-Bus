namespace Frontend.Observer
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
