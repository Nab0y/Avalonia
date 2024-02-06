using static System.Environment;

namespace Avalonia.Aurora.PathProvider;

public static class BaseDirectory
{
    public static string DataHome
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(DataHome), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = Path.Combine(HomeDirectory.Home, ".local", "share");
            Cache.SingleValueCache[nameof(DataHome)] = result;
            return result;
        }
    }

    public static string ConfigHome
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(ConfigHome), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, ".config");
            Cache.SingleValueCache[nameof(ConfigHome)] = result;
            return result;
        }
    }

    public static string StateHome
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(StateHome), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, ".local", "state");
            Cache.SingleValueCache[nameof(StateHome)] = result;
            return result;
        }
    }

    public static string BinHome
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(BinHome), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = Path.Combine(HomeDirectory.Home, ".local", "bin");
            Cache.SingleValueCache[nameof(BinHome)] = result;
            return result;
        }
    }

    public static IList<string> DataDirs
    {
        get
        {
            if (Cache.MultiValueCache.TryGetValue(nameof(DataDirs), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = new string[] { "/usr/local/share", "/usr/share" };
            Cache.MultiValueCache[nameof(DataDirs)] = result;
            return result;
        }
    }

    public static string ConfigDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(ConfigDir), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = "/etc/xdg";
            Cache.SingleValueCache[nameof(ConfigDir)] = result;
            return result;
        }
    }

    public static string CacheHome
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(CacheHome), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = Path.Combine(HomeDirectory.Home, ".cache");
            Cache.SingleValueCache[nameof(CacheHome)] = result;
            return result;
        }
    }

    public static string RuntimeDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(RuntimeDir), out var cacheValue))
            {
                return cacheValue;
            }
            
            var result = GetEnvironmentVariable("XDG_RUNTIME_DIR") 
                         ?? Path.Combine("/run", "user", GetEnvironmentVariable("LAST_LOGIN_UID") ?? "0");
            Cache.SingleValueCache[nameof(RuntimeDir)] = result;
            return result;
        }
    }
}
