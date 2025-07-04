namespace Backend.Utils.Loaders;

public static class JsonFileLoader
{
  public static void LoadFromFiles(string linesFilePath, string stopsFilePath)
  {
    if (!File.Exists(linesFilePath))
    {
      Console.WriteLine($"Lines JSON file not found: {linesFilePath}");
      return;
    }
    if (!File.Exists(stopsFilePath))
    {
      Console.WriteLine($"Stops JSON file not found: {stopsFilePath}");
      return;
    }

    string linesJson = File.ReadAllText(linesFilePath);
    string stopsJson = File.ReadAllText(stopsFilePath);

    BusNetworkLoader.LoadFromJsonStrings(linesJson, stopsJson);
    Console.WriteLine("Loaded bus network data from JSON files successfully.");
  }
}