namespace Avalonia.Aurora.PathProvider;

public static class UserDirectory
{
    public static string DesktopDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(DesktopDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Desktop");
            Cache.SingleValueCache[nameof(DesktopDir)] = result;
            return result;
        }
    }

    public static string DownloadDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(DownloadDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Downloads");
            Cache.SingleValueCache[nameof(DownloadDir)] = result;
            return result;
        }
    }

    public static string DocumentsDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(DocumentsDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Documents");
            Cache.SingleValueCache[nameof(DocumentsDir)] = result;
            return result;
        }
    }

    public static string MusicDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(MusicDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Music");
            Cache.SingleValueCache[nameof(MusicDir)] = result;
            return result;
        }
    }
    
    public static string PicturesDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(PicturesDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Pictures");
            Cache.SingleValueCache[nameof(PicturesDir)] = result;
            return result;
        }
    }
    
    public static string VideosDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(VideosDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Videos");
            Cache.SingleValueCache[nameof(VideosDir)] = result;
            return result;
        }
    }
    
    public static string TemplatesDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(TemplatesDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Templates");
            Cache.SingleValueCache[nameof(TemplatesDir)] = result;
            return result;
        }
    }
    
    public static string PublicDir
    {
        get
        {
            if (Cache.SingleValueCache.TryGetValue(nameof(PublicDir), out var cacheValue))
            {
                return cacheValue;
            }

            var result = Path.Combine(HomeDirectory.Home, "Public");
            Cache.SingleValueCache[nameof(PublicDir)] = result;
            return result;
        }
    }
}
