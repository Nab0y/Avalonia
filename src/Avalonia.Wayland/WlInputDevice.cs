using System;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using NWayland.Protocols.Wayland;

namespace Avalonia.Wayland
{
    internal class WlInputDevice : WlSeat.IEvents, IDisposable, ITextInputMethodImpl
    {
        private readonly AvaloniaWaylandPlatform _platform;

        public WlInputDevice(AvaloniaWaylandPlatform platform)
        {
            _platform = platform;
            _platform.WlSeat.Events = this;
        }

        public WlPointerHandler? PointerHandler { get; private set; }

        public WlKeyboardHandler? KeyboardHandler { get; private set; }

        public WlTouchHandler? TouchHandler { get; private set; }

        public RawInputModifiers RawInputModifiers { get; set; }

        public uint Serial { get; set; }

        public uint UserActionDownSerial { get; set; }

        public void OnCapabilities(WlSeat eventSender, WlSeat.CapabilityEnum capabilities)
        {
            if (capabilities.HasAllFlags(WlSeat.CapabilityEnum.Pointer))
                PointerHandler = new WlPointerHandler(_platform, this);
            if (capabilities.HasAllFlags(WlSeat.CapabilityEnum.Keyboard))
                KeyboardHandler = new WlKeyboardHandler(_platform, this);
            if (capabilities.HasAllFlags(WlSeat.CapabilityEnum.Touch))
                TouchHandler = new WlTouchHandler(_platform, this);
        }

        public void OnName(WlSeat eventSender, string name) { }

        public void Dispose()
        {
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
}
