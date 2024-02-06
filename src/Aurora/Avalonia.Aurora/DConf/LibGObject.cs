using System.Runtime.InteropServices;

namespace Avalonia.Aurora.DConf;

internal static class LibGObject
{
    private const string gObject = "libgobject-2.0.so.0";

    [DllImport(gObject, EntryPoint = "g_variant_print")]
    public static extern string GVariantPrint(IntPtr variant, bool typeAnnotate);
}
