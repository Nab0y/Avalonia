namespace Avalonia.Aurora.PathProvider;

public static class Cache
{
    public static Dictionary<string, string> SingleValueCache => new();
    public static Dictionary<string, IList<string>> MultiValueCache => new();
}
