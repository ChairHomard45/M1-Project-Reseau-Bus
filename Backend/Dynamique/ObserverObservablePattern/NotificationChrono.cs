namespace Backend.Dynamique.ObserverObservablePattern;

public class NotificationChrono
{
  private readonly TimeSpan? _dureeRestante;
  private readonly NotificationChronoType _type;
  private readonly String? _typeChrono;

  public NotificationChrono(NotificationChronoType type, String? typeChrono, TimeSpan? dureeRestante)
  {
    _type = type;
    _typeChrono = typeChrono;
    _dureeRestante = dureeRestante;
  }

  public TimeSpan? GetDureeRestante()
  {
    return _dureeRestante;
  }

  public NotificationChronoType GetTypeNotification()
  {
    return _type;
  }

  public String? GetTypeChrono()
  {
    return _typeChrono;
  }
}

public enum NotificationChronoType
{
  StartChrono,
  StopChrono,
  DureeChrono
}