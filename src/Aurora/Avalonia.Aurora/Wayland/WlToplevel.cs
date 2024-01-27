using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlToplevel : WlWindow, IWindowImpl
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;

    private WlShellSurface? _wlShellSurface;
    private string? _title;
    private PixelSize _minSize;
    private PixelSize _maxSize;
    private bool _extendIntoClientArea;
    private SystemDecorations _systemDecorations = SystemDecorations.Full;

    public WlToplevel(AvaloniaAuroraWaylandPlatform platform) : base(platform)
    {
        _platform = platform;
        
    }

    public Func<WindowCloseReason, bool>? Closing { get; set; }

    public Action? GotInputWhenDisabled { get; set; }

    public Action<WindowState>? WindowStateChanged { get; set; }

    public Action<bool>? ExtendClientAreaToDecorationsChanged { get; set; }

    public Action<SystemDecorations>? RequestedManagedDecorationsChanged { get; set; }

    public bool IsClientAreaExtendedToDecorations => !AppliedState.HasWindowDecorations;

    public SystemDecorations RequestedManagedDecorations => AppliedState.HasWindowDecorations ? SystemDecorations.None : _systemDecorations;

    private Thickness _extendedMargins = s_windowDecorationThickness;
    public Thickness ExtendedMargins => IsClientAreaExtendedToDecorations ? _extendedMargins : default;

    public Thickness OffScreenMargin => default;

    public WindowState WindowState
    {
        get => AppliedState.WindowState;
        set
        {
            if (AppliedState.WindowState == value)
                return;
            // switch (value)
            // {
            //     case WindowState.Minimized:
            //         _xdgToplevel?.SetMinimized();
            //         break;
            //     case WindowState.Maximized:
            //         _xdgToplevel?.UnsetFullscreen();
            //         _xdgToplevel?.SetMaximized();
            //         break;
            //     case WindowState.FullScreen:
            //         _xdgToplevel?.SetFullscreen(WlOutput);
            //         break;
            //     case WindowState.Normal:
            //         _xdgToplevel?.UnsetFullscreen();
            //         _xdgToplevel?.UnsetMaximized();
            //         break;
            // }
        }
    }

    public override async void Show(bool activate, bool isDialog)
    {
        _wlShellSurface = _platform.WlShell.GetShellSurface(WlSurface);
        _wlShellSurface.Configure += OnConfigure;
        _wlShellSurface.Ping += OnPing;
        _wlShellSurface.PopupDone += WlShellSurfaceOnPopupDone;
        
        _wlShellSurface.SetToplevel();
        
        if (_title is not null)
        {
            _wlShellSurface.SetTitle(_title);
        }

        //base.Show(activate, isDialog);
        await ShowAsync(activate, isDialog);
    }

    private void WlShellSurfaceOnPopupDone(object? sender, WlShellSurface.PopupDoneEventArgs e)
    {
        
    }

    private void OnPing(object? sender, WlShellSurface.PingEventArgs e)
    {
        ((WlShellSurface)sender!).Pong(e.Serial);
    }

    

    private void DisposeWlShellSurface()
    {
        if (_wlShellSurface == null)
        {
            return;
        }
        
        _wlShellSurface.PopupDone -= WlShellSurfaceOnPopupDone;
        _wlShellSurface.Configure -= OnConfigure;
        _wlShellSurface.Ping -= OnPing;
        _wlShellSurface.Dispose();
        _wlShellSurface = null;
    }
    
    public override void Hide()
    {
        WlSurface.Attach(null, 0, 0);
        DisposeWlShellSurface();
        
        _platform.WlDisplay.Roundtrip();
    }

    public void SetTitle(string? title)
    {
        _title = title;
        _wlShellSurface?.SetTitle(title ?? string.Empty);
    }

    public void SetParent(IWindowImpl parent)
    {
        if (parent is not WlToplevel wlToplevel || _wlShellSurface is null)
        {
            return;
        }

        //todo: nab0y
        _wlShellSurface.SetPopup(_platform.WlSeat, _platform.WlInputDevice.Serial, wlToplevel.WlSurface, 0, 0, (uint)WlShellSurfaceTransient.Inactive);
        
        Parent = wlToplevel;
    }

    public void SetEnabled(bool enable) { }

    public void SetSystemDecorations(SystemDecorations enabled)
    {
        _systemDecorations = enabled;
        // var extend = _extendIntoClientArea || _systemDecorations != SystemDecorations.Full;
        // var mode = extend ? ZxdgToplevelDecorationV1.ModeEnum.ClientSide : ZxdgToplevelDecorationV1.ModeEnum.ServerSide;
        // _toplevelDecoration?.SetMode(mode);
    }

    public void SetIcon(IWindowIconImpl? icon) { } // Impossible on Wayland, an AppId should be used instead.

    public void ShowTaskbarIcon(bool value) { } // Impossible on Wayland.

    public void CanResize(bool value)
    {
        //todo: nab0y
        // if (value)
        // {
        //     _xdgToplevel?.SetMinSize(_minSize.Width, _minSize.Height);
        //     _xdgToplevel?.SetMaxSize(_maxSize.Width, _maxSize.Height);
        // }
        // else
        // {
        //     _xdgToplevel?.SetMinSize(AppliedState.Size.Width, AppliedState.Size.Height);
        //     _xdgToplevel?.SetMaxSize(AppliedState.Size.Width, AppliedState.Size.Height);
        // }
    }

    public void BeginMoveDrag(PointerPressedEventArgs e)
    {
        _wlShellSurface?.Move(_platform.WlSeat, _platform.WlInputDevice.Serial);
        //_xdgToplevel?.Move(_platform.WlSeat, _platform.WlInputDevice.Serial);
        e.Pointer.Capture(null);
    }

    public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
    {
        _wlShellSurface?.Resize(_platform.WlSeat, _platform.WlInputDevice.Serial, (uint)ParseWindowEdges(edge));
        //_xdgToplevel?.Resize(_platform.WlSeat, _platform.WlInputDevice.Serial, ParseWindowEdges(edge));
        e.Pointer.Capture(null);
    }

    public void Move(PixelPoint point) { } // Impossible on Wayland.

    public void SetMinMaxSize(Size minSize, Size maxSize)
    {
        var minX = double.IsInfinity(minSize.Width) ? 0 : (int)minSize.Width;
        var minY = double.IsInfinity(minSize.Height) ? 0 : (int)minSize.Height;
        var maxX = double.IsInfinity(maxSize.Width) ? 0 : (int)maxSize.Width;
        var maxY = double.IsInfinity(maxSize.Height) ? 0 : (int)maxSize.Height;
        _minSize = new PixelSize(minX, minY);
        _maxSize = new PixelSize(maxX, maxY);
    }

    public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
    {
        _extendIntoClientArea = extendIntoClientAreaHint;
        //var mode = extendIntoClientAreaHint ? ZxdgToplevelDecorationV1.ModeEnum.ClientSide : ZxdgToplevelDecorationV1.ModeEnum.ServerSide;
        //_toplevelDecoration?.SetMode(mode);
    }

    public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints) { }

    public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
    {
        _extendedMargins = titleBarHeight is -1 ? s_windowDecorationThickness : new Thickness(0, titleBarHeight, 0, 0);
    }

    private void OnConfigure(object? sender, WlShellSurface.ConfigureEventArgs e)
    {
        
        var size = new PixelSize(e.Width, e.Height);
        if (size != default)
            PendingState.Size = size;
    }
    
    // public void OnConfigure(XdgToplevel eventSender, int width, int height, ReadOnlySpan<XdgToplevel.StateEnum> states)
    // {
    //     PendingState.WindowState = WindowState.Normal;
    //     foreach (var state in states)
    //     {
    //         switch (state)
    //         {
    //             case XdgToplevel.StateEnum.Maximized:
    //                 PendingState.WindowState = WindowState.Maximized;
    //                 break;
    //             case XdgToplevel.StateEnum.Fullscreen:
    //                 PendingState.WindowState = WindowState.FullScreen;
    //                 break;
    //             case XdgToplevel.StateEnum.Activated:
    //                 PendingState.Activated = true;
    //                 break;
    //         }
    //     }
    //
    //     var size = new PixelSize(width, height);
    //     if (size != default)
    //         PendingState.Size = size;
    // }
    
    
    public override object? TryGetFeature(Type featureType)
    {
        // if (featureType == typeof(IStorageProvider))
        //     return _storageProvider;
        return base.TryGetFeature(featureType);
    }

    public override void Dispose()
    {
        DisposeWlShellSurface();
        base.Dispose();
    }

    protected override void ApplyConfigure()
    {
        var windowStateChanged = PendingState.WindowState != AppliedState.WindowState;

        base.ApplyConfigure();

        if (AppliedState.Activated)
            Activated?.Invoke();
        if (windowStateChanged)
            WindowStateChanged?.Invoke(AppliedState.WindowState);

        ExtendClientAreaToDecorationsChanged?.Invoke(IsClientAreaExtendedToDecorations);
        RequestedManagedDecorationsChanged?.Invoke(RequestedManagedDecorations);
    }

    private static readonly Thickness s_windowDecorationThickness = new(0, 30, 0, 0);

    private static WlShellSurfaceResize ParseWindowEdges(WindowEdge windowEdge) => windowEdge switch
    {
        WindowEdge.North => WlShellSurfaceResize.Top,
        WindowEdge.NorthEast => WlShellSurfaceResize.TopRight,
        WindowEdge.East => WlShellSurfaceResize.Right,
        WindowEdge.SouthEast => WlShellSurfaceResize.BottomRight,
        WindowEdge.South => WlShellSurfaceResize.Bottom,
        WindowEdge.SouthWest => WlShellSurfaceResize.BottomLeft,
        WindowEdge.West => WlShellSurfaceResize.Left,
        WindowEdge.NorthWest => WlShellSurfaceResize.TopLeft,
        _ => throw new ArgumentOutOfRangeException(nameof(windowEdge))
    };
}
