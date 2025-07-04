using Backend.Dynamique.Entities;

namespace Backend.Dynamique.Observer;

public class BusTimerNotification
{
  public enum NotificationType
  {
    MoveNext,
    ChangeStateToStop,
    ChangeStateToCirculation
  }

  public NotificationType Type { get; }
  public Bus Bus { get; }
  
  public BusTimerNotification(Bus bus, NotificationType type)
  {
    Bus = bus;
    Type = type;
  }
}
