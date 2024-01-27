using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Threading;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlPointerHandler : IDisposable
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;
    private readonly ICursorFactory _cursorFactory;
    private readonly WlInputDevice _wlInputDevice;
    private readonly WlPointer _wlPointer;
    private readonly WlSurface _pointerSurface;

    private Point _pointerPosition;
    private int _currentCursorImageIndex;
    private WlCursor? _currentCursor;
    private IDisposable? _pointerTimer;

    private WlWindow? _pointerWindow;

    public WlPointerHandler(AvaloniaAuroraWaylandPlatform platform, WlInputDevice wlInputDevice)
    {
        _platform = platform;
        _wlInputDevice = wlInputDevice;
        _wlPointer = platform.WlSeat.GetPointer();
        _wlPointer.Enter += OnEnter;
        _wlPointer.Leave += OnLeave;
        _wlPointer.Axis += OnAxis;
        _wlPointer.Button += OnButton;
        _wlPointer.Motion += OnMotion;

        _cursorFactory = AvaloniaLocator.Current.GetRequiredService<ICursorFactory>();
        _pointerSurface = platform.WlCompositor.CreateSurface();
        MouseDevice = new MouseDevice();
    }

    public void Dispose()
    {
        _wlPointer.Enter -= OnEnter;
        _wlPointer.Leave -= OnLeave;
        _wlPointer.Axis -= OnAxis;
        _wlPointer.Button -= OnButton;
        _wlPointer.Motion -= OnMotion;
        
        _wlPointer.Dispose();
        _pointerSurface.Dispose();
        _currentCursor?.Dispose();
        _pointerTimer?.Dispose();
        MouseDevice.Dispose();
    }
    
    private void OnEnter(object? sender, WlPointer.EnterEventArgs e)
    {
        _wlInputDevice.Serial = e.Serial;
        PointerSurfaceSerial = e.Serial;
        _pointerWindow = _platform.WlScreens.WindowFromSurface(e.Surface);
        _pointerPosition = new Point(e.SurfaceX, e.SurfaceY);
        if (_pointerWindow?.InputRoot is null)
            return;
        var args = new RawPointerEventArgs(MouseDevice, 0, _pointerWindow.InputRoot, RawPointerEventType.Move, _pointerPosition, _wlInputDevice.RawInputModifiers);
        _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
    }
    
    private void OnMotion(object? sender, WlPointer.MotionEventArgs e)
    {
        _pointerPosition = new Point(e.SurfaceX, e.SurfaceY);
        if (_pointerWindow?.InputRoot is null)
            return;
        var args = new RawPointerEventArgs(MouseDevice, e.Time, _pointerWindow.InputRoot, RawPointerEventType.Move, _pointerPosition, _wlInputDevice.RawInputModifiers);
        _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
    }

    private void OnLeave(object? sender, WlPointer.LeaveEventArgs e)
    {
        var window = _platform.WlScreens.WindowFromSurface(e.Surface);
        _pointerWindow = null;
        _currentCursor = null;
        PointerSurfaceSerial = e.Serial;
        if (window?.InputRoot is null)
            return;
        var args = new RawPointerEventArgs(MouseDevice, 0, window.InputRoot, RawPointerEventType.LeaveWindow, _pointerPosition, _wlInputDevice.RawInputModifiers);
        _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(window, args));
    }

    private void OnButton(object? sender, WlPointer.ButtonEventArgs e)
    {
        _wlInputDevice.Serial = e.Serial;
        RawPointerEventType type;
        var state = (WlPointerButtonState)e.State;
        switch (e.Button)
        {
            case (uint)EvKey.BTN_LEFT when state == WlPointerButtonState.Pressed:
                type = RawPointerEventType.LeftButtonDown;
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.LeftMouseButton;
                _wlInputDevice.UserActionDownSerial = e.Serial;
                break;
            case (uint)EvKey.BTN_LEFT when state == WlPointerButtonState.Released:
                type = RawPointerEventType.LeftButtonUp;
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.LeftMouseButton;
                break;
            case (uint)EvKey.BTN_RIGHT when state == WlPointerButtonState.Pressed:
                type = RawPointerEventType.RightButtonDown;
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.RightMouseButton;
                _wlInputDevice.UserActionDownSerial = e.Serial;
                break;
            case (uint)EvKey.BTN_RIGHT when state == WlPointerButtonState.Released:
                type = RawPointerEventType.RightButtonUp;
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.RightMouseButton;
                break;
            case (uint)EvKey.BTN_MIDDLE when state == WlPointerButtonState.Pressed:
                type = RawPointerEventType.MiddleButtonDown;
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.MiddleMouseButton;
                _wlInputDevice.UserActionDownSerial = e.Serial;
                break;
            case (uint)EvKey.BTN_MIDDLE when state == WlPointerButtonState.Released:
                type = RawPointerEventType.MiddleButtonUp;
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.MiddleMouseButton;
                break;
            case (uint)EvKey.BTN_SIDE when state == WlPointerButtonState.Pressed:
                type = RawPointerEventType.XButton2Down;
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.XButton2MouseButton;
                _wlInputDevice.UserActionDownSerial = e.Serial;
                break;
            case (uint)EvKey.BTN_SIDE when state == WlPointerButtonState.Released:
                type = RawPointerEventType.XButton2Up;
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.XButton2MouseButton;
                break;
            case (uint)EvKey.BTN_EXTRA when state == WlPointerButtonState.Pressed:
                type = RawPointerEventType.XButton1Down;
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.XButton1MouseButton;
                _wlInputDevice.UserActionDownSerial = e.Serial;
                break;
            case (uint)EvKey.BTN_EXTRA when state == WlPointerButtonState.Released:
                type = RawPointerEventType.XButton1Up;
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.XButton1MouseButton;
                break;
            default:
                return;
        }

        if (_pointerWindow?.InputRoot is null)
            return;
        var args = new RawPointerEventArgs(MouseDevice, e.Time, _pointerWindow.InputRoot, type, _pointerPosition, _wlInputDevice.RawInputModifiers);
        _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
    }

    private void OnAxis(object? sender, WlPointer.AxisEventArgs e)
    {
        if (_pointerWindow?.InputRoot is null)
            return;
        const double scrollFactor = 0.1;
        var scrollValue = -e.Value * scrollFactor;
        var axis = (WlPointerAxis)e.Axis;
        var delta = axis == WlPointerAxis.HorizontalScroll ? new Vector(scrollValue, 0) : new Vector(0, scrollValue);
        var args = new RawMouseWheelEventArgs(MouseDevice, e.Time, _pointerWindow.InputRoot, _pointerPosition, delta, _wlInputDevice.RawInputModifiers);
        _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
    }

    public MouseDevice MouseDevice { get; }

    public uint PointerSurfaceSerial { get; private set; }

    public void SetCursor(WlCursor? wlCursor)
    {
        wlCursor ??= _cursorFactory.GetCursor(StandardCursorType.Arrow) as WlCursor;
        if (wlCursor is null || wlCursor.ImageCount <= 0  || _currentCursor == wlCursor)
            return;
        _pointerTimer?.Dispose();
        _currentCursor = wlCursor;
        _currentCursorImageIndex = -1;
        if (wlCursor.ImageCount == 1)
            SetCursorImage(wlCursor[0]);
        else
            _pointerTimer = DispatcherTimer.Run(OnCursorAnimation, wlCursor[0].Delay);
    }

    

    internal void InvalidateFocus(WlWindow window)
    {
        if (_pointerWindow == window)
        {
            _pointerWindow = null;
        }
        // if (_pinchGestureWindow == window)
        //     _pinchGestureWindow = null;
        // if (_swipeGestureWindow == window)
        //     _swipeGestureWindow = null;
    }

    private bool OnCursorAnimation()
    {
        var oldImage = _currentCursorImageIndex == -1 ? null : _currentCursor![_currentCursorImageIndex];
        if (++_currentCursorImageIndex >= _currentCursor!.ImageCount)
            _currentCursorImageIndex = 0;
        var newImage = _currentCursor[_currentCursorImageIndex];
        SetCursorImage(newImage);
        if (oldImage is null || oldImage.Delay == newImage.Delay)
            return true;
        _pointerTimer?.Dispose();
        _pointerTimer = DispatcherTimer.Run(OnCursorAnimation, newImage.Delay);
        return false;
    }

    private void SetCursorImage(WlCursor.WlCursorImage cursorImage)
    {
        _pointerSurface.Attach(cursorImage.WlBuffer, 0, 0);
        _pointerSurface.Damage(0, 0, cursorImage.Size.Width, cursorImage.Size.Height);
        _pointerSurface.Commit();
        _wlPointer.SetCursor(PointerSurfaceSerial, _pointerSurface, cursorImage.Hotspot.X, cursorImage.Hotspot.Y);
    }
}
