using System.Runtime.InteropServices;

namespace Avalonia.Aurora.DConf;

internal static class LibGLib
{
    //https://github.com/mono/gtk-sharp/blob/dadc19cf1b90c5743f2776c675faac990e397a56/glib/MainLoop.cs#L29
    
    private const string gLib = "libglib-2.0.so.0";
    
    [DllImport (gLib, EntryPoint = "g_main_loop_new", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr MainLoopNew (IntPtr context, bool isRunning);
    
    [DllImport (gLib, EntryPoint = "g_main_loop_unref", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MainLoopUnref (IntPtr loop);
    
    [DllImport (gLib, EntryPoint = "g_main_loop_is_running",CallingConvention = CallingConvention.Cdecl)]
    public static extern bool MainLoopIsRunning (IntPtr loop);
    
    [DllImport (gLib, EntryPoint = "g_main_loop_run", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MainLoopRun (IntPtr loop);
    
    [DllImport (gLib, EntryPoint  = "g_main_loop_quit", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MainLoopQuit (IntPtr loop);
    
    [DllImport (gLib, EntryPoint = "g_main_loop_get_context", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr MainLoopGetContext (IntPtr loop);
}
