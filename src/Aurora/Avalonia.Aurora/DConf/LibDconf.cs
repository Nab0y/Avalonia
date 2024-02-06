using System.Runtime.InteropServices;

namespace Avalonia.Aurora.DConf;

internal static class LibDconf
{
    private const string libDconf = "libdconf.so.1";

    [DllImport(libDconf, EntryPoint = "dconf_client_new")]
    public static extern IntPtr NewClient();

    [DllImport(libDconf, EntryPoint = "dconf_client_read")]
    public static extern IntPtr Read(IntPtr client, string key);
    
    [DllImport(libDconf, EntryPoint = "dconf_client_write_sync")]
    public static extern bool WriteSync(IntPtr client, string key, IntPtr value,
        ref string tag, IntPtr cancellable, out IntPtr error);
}
