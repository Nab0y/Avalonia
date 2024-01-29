using Avalonia.Aurora.Wayland.Egl;
using Avalonia.Aurora.Wayland.Framebuffer;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using NWayland.Protocols.Aurora.Wayland;

namespace Avalonia.Aurora.Wayland;

internal abstract class WlWindow : IWindowBaseImpl, WlSurface.IEvents
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;
    private readonly WlFramebufferSurface _wlFramebufferSurface;

    private readonly WlEglGlPlatformSurface? _wlEglGlPlatformSurface;

    //private readonly WpViewport? _wpViewport;
    // private readonly WpFractionalScaleV1? _wpFractionalScale;
    private readonly object _resizeLock = new();

    private bool _didReceiveInitialConfigure;
    private WlCallback? _frameCallback;

    internal State PendingState = default;
    internal State AppliedState = default;

    internal struct State
    {
        public int VisibleState;
        public PixelSize Size = new(720, 1280);
        public PixelPoint Position = default;
        public WindowState WindowState = WindowState.FullScreen;
        public bool HasWindowDecorations = false;
        public bool Activated = false;

        public State()
        {
        }
    }

    protected WlWindow(AvaloniaAuroraWaylandPlatform platform)
    {
        _platform = platform;
        WlSurface = platform.WlCompositor.CreateSurface();
        WlSurface.Events = this;

        var surfaces = new List<object>(2);

        var platformGraphics = AvaloniaLocator.Current.GetService<IPlatformGraphics>();
        if (platformGraphics is EglPlatformGraphics)
        {
            var surfaceInfo = new WlEglSurfaceInfo(this);
            _wlEglGlPlatformSurface = new WlEglGlPlatformSurface(surfaceInfo);
            surfaces.Add(_wlEglGlPlatformSurface);
        }

        _wlFramebufferSurface = new WlFramebufferSurface(_platform, this);
        surfaces.Add(_wlFramebufferSurface);

        Surfaces = surfaces.ToArray();

        platform.WlScreens.AddWindow(this);
    }

    public IPlatformHandle Handle => null!;

    public Size MaxAutoSizeHint
    {
        get
        {
            //AppliedState.Bounds != default ? AppliedState.Bounds.ToSize(1) : Size.Infinity;
            return Size.Infinity;
        }
    }

    public Size ClientSize => AppliedState.Size.ToSize(RenderScaling);

    public Size? FrameSize => null;

    public PixelPoint Position => AppliedState.Position;

    public double RenderScaling { get; private set; } = 1;

    public double DesktopScaling => RenderScaling;

    public WindowTransparencyLevel TransparencyLevel { get; private set; } = WindowTransparencyLevel.None;

    public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => default;

    public Compositor Compositor => _platform.Compositor;

    public IScreenImpl Screen => _platform.WlScreens;

    public IEnumerable<object> Surfaces { get; }

    public Action<RawInputEventArgs>? Input { get; set; }

    public Action<Rect>? Paint { get; set; }

    public Action<Size, WindowResizeReason>? Resized { get; set; }

    public Action<double>? ScalingChanged { get; set; }

    public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

    public Action? Activated { get; set; }

    public Action? Deactivated { get; set; }

    public Action? LostFocus { get; set; }

    public Action? Closed { get; set; }

    public Action<PixelPoint>? PositionChanged { get; set; }

    internal IInputRoot? InputRoot { get; private set; }

    internal WlSurface WlSurface { get; }

    internal WlOutput? WlOutput { get; private set; }

    protected WlWindow? Parent { get; set; }

    public void SetInputRoot(IInputRoot inputRoot) => InputRoot = inputRoot;

    public Point PointToClient(PixelPoint point) =>
        new((point.X - Position.X) / RenderScaling, (point.Y - Position.Y) / RenderScaling);

    public PixelPoint PointToScreen(Point point) => new((int)(point.X * RenderScaling + Position.X),
        (int)(point.Y * RenderScaling + Position.Y));

    public void SetCursor(ICursorImpl? cursor) => _platform.WlInputDevice.PointerHandler?.SetCursor(cursor as WlCursor);

    public IPopupImpl CreatePopup() => new WlPopup(_platform, this);

    public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
    {
        foreach (var transparencyLevel in transparencyLevels)
        {
            if (transparencyLevel == TransparencyLevel)
                return;
            if (!TryApplyTransparencyLevel(transparencyLevel))
                continue;
            TransparencyLevel = transparencyLevel;
            TransparencyLevelChanged?.Invoke(transparencyLevel);
            break;
        }
    }

    public void SetFrameThemeVariant(PlatformThemeVariant themeVariant) { }

    public virtual void Show(bool activate, bool isDialog)
    {
        WlSurface.Commit();
        _platform.WlDisplay.Roundtrip();
    }

    public abstract void Hide();

    public void Activate() { }

    public void SetTopmost(bool value) { } // impossible on Wayland

    public void Resize(Size clientSize, WindowResizeReason reason = WindowResizeReason.Application)
    {
        if (!_didReceiveInitialConfigure && clientSize != default)
        {
            AppliedState.Size = PendingState.Size = PixelSize.FromSize(clientSize, RenderScaling);
        }
    }

    public virtual object? TryGetFeature(Type featureType)
    {
        if (featureType == typeof(IClipboard))
            return _platform.WlDataHandler;
        if (featureType == typeof(ITextInputMethodImpl))
            return _platform.WlInputDevice;
        return null;
    }

    protected virtual void ApplyConfigure()
    {
        var didResize = AppliedState.Size != PendingState.Size;

        AppliedState = PendingState;

        if (!_didReceiveInitialConfigure)
        {
            // Emulate Window 7+'s default window size behavior in case no explicit size was set. If no configure_bounds event was send, fall back to a hardcoded size.
            if (AppliedState.Size == default)
            {
                // AppliedState.Size = new PixelSize(Math.Max((int)(AppliedState.Bounds.Width * 0.75), 300),
                //     Math.Max((int)(AppliedState.Bounds.Height * 0.7), 200));
                AppliedState.Size = new PixelSize(720, 1280);
            }

            _didReceiveInitialConfigure = true;
            DoPaint();
        }
        else if (didResize && _frameCallback is null)
        {
            _frameCallback = WlSurface.Frame();
            DoPaint();
        }
    }

    private void DisposeFrameCallback()
    {
        if (_frameCallback == null)
        {
            return;
        }

        _frameCallback.Dispose();
        _frameCallback = null;
    }

    public virtual void Dispose()
    {
        Closed?.Invoke();
        _platform.WlScreens.RemoveWindow(this);
        DisposeFrameCallback();
        _wlFramebufferSurface.Dispose();
        _wlEglGlPlatformSurface?.Dispose();

        WlSurface.Dispose();
    }

    private void DoPaint()
    {
        lock (_resizeLock)
        {
            Resized?.Invoke(ClientSize, WindowResizeReason.Application);
            TryApplyTransparencyLevel(TransparencyLevel);
            Paint?.Invoke(default);
        }
    }

    private bool TryApplyTransparencyLevel(WindowTransparencyLevel transparencyLevel)
    {
        if (transparencyLevel == WindowTransparencyLevel.None)
        {
            using var region = _platform.WlCompositor.CreateRegion();
            region.Add(0, 0, AppliedState.Size.Width, AppliedState.Size.Height);
            WlSurface.SetOpaqueRegion(region);
            return true;
        }

        if (transparencyLevel == WindowTransparencyLevel.Transparent)
        {
            WlSurface.SetOpaqueRegion(null);
            return true;
        }

        return false;
    }

    public void OnEnter(WlSurface eventSender, WlOutput output)
    {
        Console.WriteLine("WlSurface OnEnter");

        if (WlOutput != null && WlOutput.Equals(output))
        {
            WlOutput = null;
        }
    }

    public void OnLeave(WlSurface eventSender, WlOutput output)
    {
        Console.WriteLine("WlSurface OnLeave");

        WlOutput = output;
    }
}
