using static System.Environment;

namespace Avalonia.Aurora.PathProvider;

public static class HomeDirectory
{
    public static string Home
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(Home), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = GetEnvironmentVariable("HOME") 
                   ?? GetFolderPath(SpecialFolder.UserProfile);
            Cache.SingleValueCache[nameof(Home)] = result;
            return result;
        }        
    }

    public static IList<string> Applications
    {
        get
        {
            if (Cache.MultiValueCache.TryGetValue(nameof(Applications), out var cacheValue))
            {
                return cacheValue;
            }

            var result = new[]
            {
                $"{BaseDirectory.DataHome}/applications", $"{Home}/.local/share/applications",
                "/usr/local/share/applications", "/usr/share/applications"
            };
            Cache.MultiValueCache[nameof(Applications)] = result;
            return result;
        }
    }

    public static IList<string> Fonts
    {
        get
        {
            if (Cache.MultiValueCache.TryGetValue(nameof(Fonts), out var cacheValue))
            {
                return cacheValue;
            }

            var result = new[]
            {
                Path.Combine(BaseDirectory.DataHome, "fonts"),
                Path.Combine(Home, ".fonts"),
                Path.Combine(Home, ".local", "share", "fonts"),
                "/usr/local/share/fonts",
                "/usr/share/fonts",
            };
            Cache.MultiValueCache[nameof(Fonts)] = result;
            return result;
        }
    }
}
