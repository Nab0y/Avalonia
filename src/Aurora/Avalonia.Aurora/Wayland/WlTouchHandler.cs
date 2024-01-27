using Avalonia.Input;
using Avalonia.Input.Raw;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlTouchHandler : IDisposable
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;
    private readonly WlInputDevice _wlInputDevice;
    private readonly WlTouch _wlTouch;
    private readonly Dictionary<WlTouch, int> _touchIds;

    private Point _touchPosition;
    private WlWindow? _window;

    public WlTouchHandler(AvaloniaAuroraWaylandPlatform platform, WlInputDevice wlInputDevice)
    {
        _platform = platform;
        _wlInputDevice = wlInputDevice;
        _wlTouch = platform.WlSeat.GetTouch();
        _wlTouch.Cancel += OnCancel;
        _wlTouch.Down += OnDown;
        _wlTouch.Frame += OnFrame;
        _wlTouch.Motion += OnMotion;
        _wlTouch.Up += OnUp;
        
        _touchIds = new Dictionary<WlTouch, int>();
        TouchDevice = new TouchDevice();
    }

    public void Dispose()
    {
        _wlTouch.Cancel -= OnCancel;
        _wlTouch.Down -= OnDown;
        _wlTouch.Frame -= OnFrame;
        _wlTouch.Motion -= OnMotion;
        _wlTouch.Up -= OnUp;
        
        _wlTouch.Dispose();
        TouchDevice.Dispose();
    }
    
    private void OnUp(object? sender, WlTouch.UpEventArgs e)
    {
        _wlInputDevice.Serial = e.Serial;
        _touchIds.Remove((WlTouch)sender!);
        if (_window?.InputRoot is null)
            return;
        var args = new RawTouchEventArgs(TouchDevice, e.Time, _window.InputRoot, RawPointerEventType.TouchEnd, _touchPosition, _wlInputDevice.RawInputModifiers, e.Id);
        _window.Input?.Invoke(args);
    }

    private void OnMotion(object? sender, WlTouch.MotionEventArgs e)
    {
        if (_window?.InputRoot is null)
            return;
        _touchPosition = new Point(e.X, e.Y) / _window.RenderScaling;
        var args = new RawTouchEventArgs(TouchDevice, e.Time, _window.InputRoot, RawPointerEventType.TouchUpdate, _touchPosition, _wlInputDevice.RawInputModifiers, e.Id);
        _window.Input?.Invoke(args);
    }

    private void OnFrame(object? sender, WlTouch.FrameEventArgs e)
    {
    }

    private void OnDown(object? sender, WlTouch.DownEventArgs e)
    {
        _wlInputDevice.Serial = e.Serial;
        _wlInputDevice.UserActionDownSerial = e.Serial;
        _touchIds.Add((WlTouch)sender!, e.Id);
        _window = _platform.WlScreens.WindowFromSurface(e.Surface);
        if (_window?.InputRoot is null)
            return;
        _touchPosition = new Point(e.X, e.Y) / _window.RenderScaling;
        var args = new RawTouchEventArgs(TouchDevice, e.Time, _window.InputRoot, RawPointerEventType.TouchBegin, _touchPosition, _wlInputDevice.RawInputModifiers, e.Id);
        _window.Input?.Invoke(args);
    }

    private void OnCancel(object? sender, WlTouch.CancelEventArgs e)
    {
        if (_window?.InputRoot is null || !_touchIds.TryGetValue((WlTouch)sender!, out var id))
            return;
        var args = new RawTouchEventArgs(TouchDevice, 0, _window.InputRoot, RawPointerEventType.TouchCancel, _touchPosition, _wlInputDevice.RawInputModifiers, id);
        _window.Input?.Invoke(args);
    }

    public TouchDevice TouchDevice { get; }


    internal void InvalidateFocus(WlWindow window)
    {
        if (_window == window)
            _window = null;
    }
}
