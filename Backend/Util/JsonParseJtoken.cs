using Newtonsoft.Json.Linq;

namespace Backend.Util;

public static class JsonParseJtoken
{
  public static string ParseStringFromJtoken(JToken token, string key)
  {
    if (token[key] == null) return "";

    return token[key]?.ToString() ?? string.Empty;
  }

  public static int ParseIntFromJtoken(JToken token, string key)
  {
    if (token[key] == null) return 0;

    return int.Parse(token[key]?.ToString() ?? string.Empty);
  }

  public static float ParseFloatFromJtoken(JToken token, string key)
  {
    if (token[key] == null) return 0.0f;

    return float.Parse(token[key]?.ToString().Replace("f", "") ?? string.Empty);
  }
}