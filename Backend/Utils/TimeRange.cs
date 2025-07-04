namespace Backend.Utils;

public record TimeRange(TimeSpan Start, TimeSpan End)
{
  public bool Contains(TimeSpan time) => time >= Start && time < End;
}

