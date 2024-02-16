using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Aurora.DConf;

internal sealed class MainLoop : IDisposable
{
    private IntPtr _handle;
    
    public MainLoop()
    {
        _handle = LibGLib.MainLoopNew(IntPtr.Zero, false);
    }
    
    ~MainLoop()
    {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources()
    {
        LibGLib.MainLoopUnref(_handle);
        _handle = IntPtr.Zero;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    public bool IsRunning  => LibGLib.MainLoopIsRunning(_handle);

    public void Run()
    {
        LibGLib.MainLoopRun(_handle);
    }

    public void Quit()
    {
        LibGLib.MainLoopQuit(_handle);
    }
}
