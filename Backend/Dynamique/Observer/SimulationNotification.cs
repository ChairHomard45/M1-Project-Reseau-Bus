namespace Backend.Dynamique.Observer;

public class SimulationNotification
{
  public enum NotificationType
  {
    Start,
    Stop,
    CurrentTime
  }

  public NotificationType Type { get; }
  public TimeSpan? CurrentTime { get; }

  public SimulationNotification(NotificationType type, TimeSpan? currentTime = null)
  {
    Type = type;
    CurrentTime = currentTime;
  }
}
