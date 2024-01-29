using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Threading;
using NWayland.Interop;
using NWayland.Protocols.Aurora.Wayland;

namespace Avalonia.Aurora.Wayland
{
    internal class WlPointerHandler : WlPointer.IEvents, IDisposable
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
        private WlWindow? _pinchGestureWindow;
        private WlWindow? _swipeGestureWindow;

        public WlPointerHandler(AvaloniaAuroraWaylandPlatform platform, WlInputDevice wlInputDevice)
        {
            _platform = platform;
            _wlInputDevice = wlInputDevice;
            _wlPointer = platform.WlSeat.GetPointer();
            _wlPointer.Events = this;
            _cursorFactory = AvaloniaLocator.Current.GetRequiredService<ICursorFactory>();
            _pointerSurface = platform.WlCompositor.CreateSurface();
            MouseDevice = new MouseDevice();
        }

        public MouseDevice MouseDevice { get; }

        public uint PointerSurfaceSerial { get; private set; }

        public void OnEnter(WlPointer eventSender, uint serial, WlSurface surface, WlFixed surfaceX, WlFixed surfaceY)
        {
            _wlInputDevice.Serial = serial;
            PointerSurfaceSerial = serial;
            _pointerWindow = _platform.WlScreens.WindowFromSurface(surface);
            _pointerPosition = new Point((double)surfaceX, (double)surfaceY);
            if (_pointerWindow?.InputRoot is null)
                return;
            var args = new RawPointerEventArgs(MouseDevice, 0, _pointerWindow.InputRoot, RawPointerEventType.Move, _pointerPosition, _wlInputDevice.RawInputModifiers);
            _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
        }

        public void OnLeave(WlPointer eventSender, uint serial, WlSurface surface)
        {
            var window = _platform.WlScreens.WindowFromSurface(surface);
            _pointerWindow = null;
            _currentCursor = null;
            PointerSurfaceSerial = serial;
            if (window?.InputRoot is null)
                return;
            var args = new RawPointerEventArgs(MouseDevice, 0, window.InputRoot, RawPointerEventType.LeaveWindow, _pointerPosition, _wlInputDevice.RawInputModifiers);
            _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(window, args));
        }

        public void OnMotion(WlPointer eventSender, uint time, WlFixed surfaceX, WlFixed surfaceY)
        {
            _pointerPosition = new Point((double)surfaceX, (double)surfaceY);
            if (_pointerWindow?.InputRoot is null)
                return;
            var args = new RawPointerEventArgs(MouseDevice, time, _pointerWindow.InputRoot, RawPointerEventType.Move, _pointerPosition, _wlInputDevice.RawInputModifiers);
            _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
        }

        public void OnButton(WlPointer eventSender, uint serial, uint time, uint button, uint st)
        {
            var state = (WlPointer.ButtonStateEnum)st;
            _wlInputDevice.Serial = serial;
            RawPointerEventType type;
            switch (button)
            {
                case (uint)EvKey.BTN_LEFT when state == WlPointer.ButtonStateEnum.Pressed:
                    type = RawPointerEventType.LeftButtonDown;
                    _wlInputDevice.RawInputModifiers |= RawInputModifiers.LeftMouseButton;
                    _wlInputDevice.UserActionDownSerial = serial;
                    break;
                case (uint)EvKey.BTN_LEFT when state == WlPointer.ButtonStateEnum.Released:
                    type = RawPointerEventType.LeftButtonUp;
                    _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.LeftMouseButton;
                    break;
                case (uint)EvKey.BTN_RIGHT when state == WlPointer.ButtonStateEnum.Pressed:
                    type = RawPointerEventType.RightButtonDown;
                    _wlInputDevice.RawInputModifiers |= RawInputModifiers.RightMouseButton;
                    _wlInputDevice.UserActionDownSerial = serial;
                    break;
                case (uint)EvKey.BTN_RIGHT when state == WlPointer.ButtonStateEnum.Released:
                    type = RawPointerEventType.RightButtonUp;
                    _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.RightMouseButton;
                    break;
                case (uint)EvKey.BTN_MIDDLE when state == WlPointer.ButtonStateEnum.Pressed:
                    type = RawPointerEventType.MiddleButtonDown;
                    _wlInputDevice.RawInputModifiers |= RawInputModifiers.MiddleMouseButton;
                    _wlInputDevice.UserActionDownSerial = serial;
                    break;
                case (uint)EvKey.BTN_MIDDLE when state == WlPointer.ButtonStateEnum.Released:
                    type = RawPointerEventType.MiddleButtonUp;
                    _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.MiddleMouseButton;
                    break;
                case (uint)EvKey.BTN_SIDE when state == WlPointer.ButtonStateEnum.Pressed:
                    type = RawPointerEventType.XButton2Down;
                    _wlInputDevice.RawInputModifiers |= RawInputModifiers.XButton2MouseButton;
                    _wlInputDevice.UserActionDownSerial = serial;
                    break;
                case (uint)EvKey.BTN_SIDE when state == WlPointer.ButtonStateEnum.Released:
                    type = RawPointerEventType.XButton2Up;
                    _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.XButton2MouseButton;
                    break;
                case (uint)EvKey.BTN_EXTRA when state == WlPointer.ButtonStateEnum.Pressed:
                    type = RawPointerEventType.XButton1Down;
                    _wlInputDevice.RawInputModifiers |= RawInputModifiers.XButton1MouseButton;
                    _wlInputDevice.UserActionDownSerial = serial;
                    break;
                case (uint)EvKey.BTN_EXTRA when state == WlPointer.ButtonStateEnum.Released:
                    type = RawPointerEventType.XButton1Up;
                    _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.XButton1MouseButton;
                    break;
                default:
                    return;
            }

            if (_pointerWindow?.InputRoot is null)
                return;
            var args = new RawPointerEventArgs(MouseDevice, time, _pointerWindow.InputRoot, type, _pointerPosition, _wlInputDevice.RawInputModifiers);
            _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
        }

        public void OnAxis(WlPointer eventSender, uint time, uint ax, WlFixed value)
        {
            if (_pointerWindow?.InputRoot is null)
                return;

            var axis = (WlPointer.AxisEnum)ax;
            const double scrollFactor = 0.1;
            var scrollValue = -(double)value * scrollFactor;
            var delta = axis == WlPointer.AxisEnum.HorizontalScroll ? new Vector(scrollValue, 0) : new Vector(0, scrollValue);
            var args = new RawMouseWheelEventArgs(MouseDevice, time, _pointerWindow.InputRoot, _pointerPosition, delta, _wlInputDevice.RawInputModifiers);
            _platform.WlRawEventGrouper.HandleEvent(new RawPointerEventArgsWithWindow(_pointerWindow, args));
        }

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

        public void Dispose()
        {
            _wlPointer.Dispose();
            _pointerSurface.Dispose();
            _currentCursor?.Dispose();
            _pointerTimer?.Dispose();
            MouseDevice.Dispose();
        }

        internal void InvalidateFocus(WlWindow window)
        {
            if (_pointerWindow == window)
                _pointerWindow = null;
            if (_pinchGestureWindow == window)
                _pinchGestureWindow = null;
            if (_swipeGestureWindow == window)
                _swipeGestureWindow = null;
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
}
