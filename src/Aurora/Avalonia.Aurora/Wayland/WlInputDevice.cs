using Avalonia.Input;
using Avalonia.Input.TextInput;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlInputDevice : IDisposable, ITextInputMethodImpl
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;

    public WlInputDevice(AvaloniaAuroraWaylandPlatform platform)
    {
        _platform = platform;
        _platform.WlSeat.Capabilities += WlSeatOnCapabilities;
    }

    private void WlSeatOnCapabilities(object? sender, WlSeat.CapabilitiesEventArgs e)
    {
        var capabilities = (WlSeatCapability)e.Capabilities;
        if (capabilities.HasAllFlags(WlSeatCapability.Pointer))
        {
            PointerHandler = new WlPointerHandler(_platform, this);
        }

        //todo: Uncomment
        // if (capabilities.HasAllFlags(WlSeatCapability.Keyboard))
        // {
        //     KeyboardHandler = new WlKeyboardHandler(_platform, this);
        // }

        if (capabilities.HasAllFlags(WlSeatCapability.Touch))
        {
            TouchHandler = new WlTouchHandler(_platform, this);
        }
    }

    public WlPointerHandler? PointerHandler { get; private set; }

    public WlKeyboardHandler? KeyboardHandler { get; private set; }

    public WlTouchHandler? TouchHandler { get; private set; }

    public RawInputModifiers RawInputModifiers { get; set; }

    public uint Serial { get; set; }

    public uint UserActionDownSerial { get; set; }

    public void Dispose()
    {
        _platform.WlSeat.Capabilities -= WlSeatOnCapabilities;
        
        PointerHandler?.Dispose();
        KeyboardHandler?.Dispose();
        TouchHandler?.Dispose();
    }

    internal void InvalidateFocus(WlWindow window)
    {
        PointerHandler?.InvalidateFocus(window);
        KeyboardHandler?.InvalidateFocus(window);
        TouchHandler?.InvalidateFocus(window);
    }

    public void SetClient(TextInputMethodClient? client)
    {
        //todo: nab0y implement
    }

    public void SetCursorRect(Rect rect)
    {
        //todo: nab0y implement
    }

    public void SetOptions(TextInputOptions options)
    {
        //todo: nab0y implement
    }

    public void Reset()
    {
        //todo: nab0y implement
    }
}
