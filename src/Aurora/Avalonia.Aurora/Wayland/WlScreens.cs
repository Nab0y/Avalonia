using Avalonia.Platform;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlScreens : IScreenImpl, IDisposable
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;
    private readonly Dictionary<uint, WlOutput> _wlOutputs = new();
    private readonly Dictionary<WlOutput, WlScreen> _wlScreens = new();
    private readonly Dictionary<WlSurface, WlWindow> _wlWindows = new();
    private readonly List<Screen> _screens = new();

    public WlScreens(AvaloniaAuroraWaylandPlatform platform)
    {
        _platform = platform;
        _platform.WlRegistryHandler.GlobalAdded += OnGlobalAdded;
        _platform.WlRegistryHandler.GlobalRemoved += OnGlobalRemoved;
    }

    public int ScreenCount => _screens.Count;

    public IReadOnlyList<Screen> AllScreens => _screens;

    public Screen? ScreenFromWindow(IWindowBaseImpl window) =>
        (window as WlWindow)?.WlOutput is { } wlOutput ? _wlScreens[wlOutput].Screen : null;

    public Screen? ScreenFromPoint(PixelPoint point) => null;

    public Screen? ScreenFromRect(PixelRect rect) => null;

    public void Dispose()
    {
        _platform.WlRegistryHandler.GlobalAdded -= OnGlobalAdded;
        _platform.WlRegistryHandler.GlobalRemoved -= OnGlobalRemoved;
        foreach (var wlScreen in _wlScreens.Values)
            wlScreen.Dispose();
    }

    internal WlWindow? WindowFromSurface(WlSurface? wlSurface) =>
        wlSurface is not null && _wlWindows.TryGetValue(wlSurface, out var wlWindow) ? wlWindow : null;

    internal void AddWindow(WlWindow window)
    {
        _wlWindows.Add(window.WlSurface, window);
    }

    internal void RemoveWindow(WlWindow window)
    {
        _platform.WlInputDevice.InvalidateFocus(window);
        _wlWindows.Remove(window.WlSurface);
    }

    private void OnGlobalAdded(WlRegistryHandler.GlobalInfo globalInfo)
    {
        if (globalInfo.Interface != WlInterface.WlOutput.Name)
        {
            return;
        }

        var wlOutput = _platform.WlRegistryHandler.BindRequiredInterface<WlOutput>(WlInterface.WlOutput.Version, globalInfo);
        _wlOutputs.Add(globalInfo.Name, wlOutput);
        var wlScreen = new WlScreen(wlOutput, _screens);
        _wlScreens.Add(wlOutput, wlScreen);
    }

    private void OnGlobalRemoved(WlRegistryHandler.GlobalInfo globalInfo)
    {
        if (globalInfo.Interface != WlInterface.WlOutput.Name ||
            !_wlOutputs.TryGetValue(globalInfo.Name, out var wlOutput) ||
            !_wlScreens.TryGetValue(wlOutput, out var wlScreen))
        {
            return;
        }

        _wlScreens.Remove(wlOutput);
        wlScreen.Dispose();
    }

    internal sealed class WlScreen : IDisposable
    {
        private readonly WlOutput _wlOutput;
        private readonly List<Screen> _screens;

        private PixelPoint _position;
        private PixelSize _size;
        private int _scaling;

        public WlScreen(WlOutput wlOutput, List<Screen> screens)
        {
            _wlOutput = wlOutput;
            _screens = screens;
            _wlOutput.Geometry += WlOutputOnGeometry;
            _wlOutput.Mode += WlOutputOnMode;
            _wlOutput.Scale += WlOutputOnScale;
            _wlOutput.Done += WlOutputOnDone;
        }

        private void WlOutputOnDone(object? sender, WlOutput.DoneEventArgs e)
        {
            if (Screen is not null)
            {
                _screens.Remove(Screen);
            }

            Screen = new Screen(_scaling, new PixelRect(_position, _size), new PixelRect(_position, _size), false);
            _screens.Add(Screen);
        }

        private void WlOutputOnScale(object? sender, WlOutput.ScaleEventArgs e)
        {
            _scaling = e.Factor;
        }

        private void WlOutputOnMode(object? sender, WlOutput.ModeEventArgs e)
        {
            if (((WlOutputMode)e.Flags).HasAllFlags(WlOutputMode.Current))
            {
                _size = new PixelSize(e.Width, e.Height);
            }
        }

        private void WlOutputOnGeometry(object? sender, WlOutput.GeometryEventArgs e)
        {
            _position = new PixelPoint(e.X, e.Y);
        }

        public Screen? Screen { get; private set; }
        
        public void Dispose()
        {
            if (Screen is not null)
            {
                _screens.Remove(Screen);
            }
            _wlOutput.Done -= WlOutputOnDone;
            _wlOutput.Geometry -= WlOutputOnGeometry;
            _wlOutput.Mode -= WlOutputOnMode;
            _wlOutput.Scale -= WlOutputOnScale;
            _wlOutput.Dispose();
        }
    }
}
