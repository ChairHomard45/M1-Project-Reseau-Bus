namespace Backend.Dynamique.Time;

public class Horaire
{
  public TimeSpan HeureDepart { get; set; }
  public TimeSpan HeureArrivee { get; set; }

  public Horaire(TimeSpan heureDepart, TimeSpan heureArrivee)
  {
    HeureDepart = heureDepart;
    HeureArrivee = heureArrivee;
  }
}