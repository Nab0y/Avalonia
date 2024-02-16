using System.Runtime.InteropServices;
using Avalonia.Platform.Interop;

namespace Avalonia.Aurora.DConf;

internal static partial class LibGObject
{
    private const string gObject = "libgobject-2.0.so.0";

    [DllImport(gObject, EntryPoint = "g_variant_print")]
    public static extern string GVariantPrint(IntPtr variant, bool typeAnnotate);
    
    [DllImport(gObject, EntryPoint = "g_object_ref")]
    private static extern void ObjectRef(IntPtr instance);
    
    [DllImport(gObject, EntryPoint = "g_object_unref")]
    public static extern void ObjectUnref(IntPtr obj);
    
    [DllImport(gObject, EntryPoint = "g_signal_connect_object")]
    private static extern ulong SignalConnectObject(IntPtr instance, Utf8Buffer signal,
        IntPtr handler, IntPtr userData, int flags);

    [DllImport(gObject, EntryPoint = "g_signal_handler_disconnect")]
    private static extern ulong SignalHandlerDisconnect(IntPtr instance, ulong connectionId);
    
    private class ConnectedSignal : IDisposable
    {
        private readonly IntPtr _instance;
        private GCHandle _handle;
        private readonly ulong _id;

        public ConnectedSignal(IntPtr instance, GCHandle handle, ulong id)
        {
            _instance = instance;
            ObjectRef(instance);
            _handle = handle;
            _id = id;
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                SignalHandlerDisconnect(_instance, _id);
                ObjectUnref(_instance);
                _handle.Free();
            }
        }
    }

    public static IDisposable ConnectSignal<T>(IntPtr obj, string name, T handler) where T : notnull
    {
        var handle = GCHandle.Alloc(handler);
        var ptr = Marshal.GetFunctionPointerForDelegate<T>(handler);
        using var utf = new Utf8Buffer(name);
        var id = SignalConnectObject(obj, utf, ptr, IntPtr.Zero, 0);
        if (id == 0)
        {
            throw new ArgumentException("Unable to connect to signal " + name);
        }

        return new ConnectedSignal(obj, handle, id);
    }
}
