using Backend.Statique;

namespace Backend.Util;

public static class MathUtil
{
  // Calculate distance using Euclidean formula
  public static double CalculateDistance(ElementLigne from, ElementLigne to)
  {
    double scaleFactor = 10;
    double dx = (to.GetXCoordinate() - from.GetXCoordinate()) * scaleFactor;
    double dy = (to.GetYCoordinate() - from.GetYCoordinate()) * scaleFactor;
    double distance = Math.Sqrt(dx * dx + dy * dy);
    
    //Console.WriteLine($"From ({from.GetXCoordinate()}, {from.GetYCoordinate()}) to ({to.GetXCoordinate()}, {to.GetYCoordinate()}) → Distance: {distance.ToString("F2")} meters");

    return distance;
  }
}