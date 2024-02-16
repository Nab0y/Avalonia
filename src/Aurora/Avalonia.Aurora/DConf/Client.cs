using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Avalonia.Aurora.DConf;

internal static class Utilities
{
    public static nint FunctionPointer<T>(T d)
        where T : notnull
    {
        return Marshal.GetFunctionPointerForDelegate<T>(d);
    }
}

internal sealed class Client : IDisposable
{
    public IntPtr Handler { get; private set; }
    private readonly IDisposable _signalConnection;

    private readonly MainLoop _mainLoop;
    //private IntPtr _mainLoopHandler;
    //private Thread? _mainLoopThread;
    
    public Client()
    {
        Handler = LibDconf.NewClient();
        _signalConnection = LibGObject.ConnectSignal<LibDconf.changed>(Handler, "changed", OnChanged);
        
        LibDconf.ClientWatchSync(Handler, "/desktop/sailfish/silica/statusbar/statusbarAsInset");

        _mainLoop = new MainLoop();
        //StartMainLoop();
    }

    // private void StartMainLoop()
    // {
    //     var tcs = new TaskCompletionSource<IntPtr>();
    //     _mainLoopThread = new Thread(() => MainLoopThread(tcs))
    //     {
    //         Name = "DConfClientMainLoop",
    //         IsBackground = true,
    //     };
    //     _mainLoopThread.Start();
    //     _mainLoopHandler = tcs.Task.Result;
    // }
    //
    // private void MainLoopThread(TaskCompletionSource<IntPtr> tcs)
    // {
    //     try
    //     {
    //         var mainLoopHandler = LibGLib.MainLoopNew(IntPtr.Zero, false);
    //         tcs.SetResult(mainLoopHandler);
    //         Console.WriteLine("MainLoop Start");
    //         LibGLib.MainLoopRun(_mainLoopHandler);
    //         Console.WriteLine("MainLoop Stop");
    //     }
    //     catch 
    //     {
    //         tcs.SetResult(IntPtr.Zero);
    //         Console.WriteLine("MainLoop exception");
    //     }
    // }
    //
    // private void TryStopMainLoop()
    // {
    //     if (_mainLoopHandler != IntPtr.Zero)
    //     {
    //         if (LibGLib.MainLoopIsRunning(_mainLoopHandler))
    //         {
    //             LibGLib.MainLoopQuit(_mainLoopHandler);     
    //         }
    //         
    //         LibGLib.MainLoopUnref(_mainLoopHandler);
    //         _mainLoopHandler = IntPtr.Zero;
    //     }
    //
    //     if (_mainLoopThread != null && _mainLoopThread.IsAlive)
    //     {
    //         _mainLoopThread.Interrupt();
    //         _mainLoopThread.Join();
    //         _mainLoopThread = null;
    //         
    //         Console.WriteLine("_mainLoopThread terminated");
    //     }
    // }

    private void OnChanged(IntPtr client, string prefix, IntPtr changes, string tag, IntPtr userData)
    {
        Console.WriteLine($"Dconf changed signal: prefix-{prefix}:changes-{changes}:tag-{tag}");
    }

    public void Dispose()
    {
        //Console.WriteLine("Client Dispose");
        //TryStopMainLoop();
        _mainLoop.Dispose();
        
        _signalConnection.Dispose();
        LibGObject.ObjectUnref(Handler);
    }
}
