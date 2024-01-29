using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NWayland.Interop;
using NWayland.Protocols.Aurora.Wayland;

#nullable enable
// <auto-generated/>
namespace NWayland.Protocols.Aurora.TouchExtension
{
    public sealed unsafe partial class QtTouchExtension : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static QtTouchExtension()
        {
            NWayland.Protocols.Aurora.TouchExtension.QtTouchExtension.WlInterface = new WlInterface("qt_touch_extension", 1, new WlMessage[] {
                new WlMessage("dummy", "", new WlInterface*[] { })
            }, new WlMessage[] {
                new WlMessage("touch", "uuuiiiiiiuiiua", new WlInterface*[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null }),
                new WlMessage("configure", "u", new WlInterface*[] { null })
            });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.TouchExtension.QtTouchExtension.WlInterface);
        }

        public void Dummy()
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
        }

        public interface IEvents
        {
            void OnTouch(NWayland.Protocols.Aurora.TouchExtension.QtTouchExtension eventSender, uint @time, uint @id, uint @state, int @x, int @y, int @normalizedX, int @normalizedY, int @width, int @height, uint @pressure, int @velocityX, int @velocityY, uint @flags, ReadOnlySpan<byte> @rawdata);
            void OnConfigure(NWayland.Protocols.Aurora.TouchExtension.QtTouchExtension eventSender, uint @flags);
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
            switch (opcode)
            {
                case 0:
                    Events?.OnTouch(this, arguments[0].UInt32, arguments[1].UInt32, arguments[2].UInt32, arguments[3].Int32, arguments[4].Int32, arguments[5].Int32, arguments[6].Int32, arguments[7].Int32, arguments[8].Int32, arguments[9].UInt32, arguments[10].Int32, arguments[11].Int32, arguments[12].UInt32, WlArray.SpanFromWlArrayPtr<byte>(arguments[13].IntPtr));
                    break;
                case 1:
                    Events?.OnConfigure(this, arguments[0].UInt32);
                    break;
            }
        }

        public enum FlagsEnum
        {
            MouseFromTouch = 0x1
        }

        private class ProxyFactory : IBindFactory<QtTouchExtension>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.TouchExtension.QtTouchExtension.WlInterface);
            }

            public QtTouchExtension Create(IntPtr handle, int version)
            {
                return new QtTouchExtension(handle, version);
            }
        }

        public static IBindFactory<QtTouchExtension> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "qt_touch_extension";
        public const int InterfaceVersion = 1;

        public QtTouchExtension(IntPtr handle, int version) : base(handle, version)
        {
        }
    }
}