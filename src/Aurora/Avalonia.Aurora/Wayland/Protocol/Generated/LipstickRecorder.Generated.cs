using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NWayland.Interop;
using NWayland.Protocols.Aurora.Wayland;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#nullable enable
// <auto-generated/>
namespace NWayland.Protocols.Aurora.LipstickRecorder
{
    public sealed unsafe partial class LipstickRecorderManager : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static LipstickRecorderManager()
        {
            NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorderManager.WlInterface = new WlInterface("lipstick_recorder_manager", 1, new WlMessage[] {
                new WlMessage("create_recorder", "no", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder.WlInterface), WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.Wayland.WlOutput.WlInterface) })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorderManager.WlInterface);
        }

        /// <summary>
        /// Create a recorder object for the specified output.
        /// </summary>
        public NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder CreateRecorder(NWayland.Protocols.Aurora.Wayland.WlOutput @output)
        {
            if (@output == null)
                throw new ArgumentNullException("output");
            WlArgument* __args = stackalloc WlArgument[] {
                WlArgument.NewId,
                @output
            };
            var __ret = LibWayland.wl_proxy_marshal_array_constructor_versioned(this.Handle, 0, __args, ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder.WlInterface, (uint)this.Version);
            return __ret == IntPtr.Zero ? null : new NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder(__ret, Version);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        private class ProxyFactory : IBindFactory<LipstickRecorderManager>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorderManager.WlInterface);
            }

            public LipstickRecorderManager Create(IntPtr handle, int version)
            {
                return new LipstickRecorderManager(handle, version);
            }
        }

        public static IBindFactory<LipstickRecorderManager> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "lipstick_recorder_manager";
        public const int InterfaceVersion = 1;

        public LipstickRecorderManager(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    public sealed unsafe partial class LipstickRecorder : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static LipstickRecorder()
        {
            NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder.WlInterface = new WlInterface("lipstick_recorder", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("record_frame", "o", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.Wayland.WlBuffer.WlInterface) }),
                new WlMessage("repaint", "", new WlInterface*[] { })
            }, new WlMessage[] {
                new WlMessage("setup", "iiii", new WlInterface*[] { null, null, null, null }),
                new WlMessage("frame", "oui", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.Wayland.WlBuffer.WlInterface), null, null }),
                new WlMessage("failed", "io", new WlInterface*[] { null, WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.Wayland.WlBuffer.WlInterface) }),
                new WlMessage("cancelled", "o", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.Wayland.WlBuffer.WlInterface) })
            });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Ask the compositor to record its next frame, putting
        /// the content into the specified buffer data. The frame
        /// event will be sent when the frame is recorded.
        /// Only one frame will be recorded, the client will have
        /// to call this again after the frame event if it wants to
        /// record more frames.
        /// 
        /// The buffer must be a shm buffer, trying to use another
        /// type of buffer will result in failure to capture the
        /// frame and the failed event will be sent.
        /// </summary>
        public void RecordFrame(NWayland.Protocols.Aurora.Wayland.WlBuffer @buffer)
        {
            if (@buffer == null)
                throw new ArgumentNullException("buffer");
            WlArgument* __args = stackalloc WlArgument[] {
                @buffer
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 1, __args);
        }

        /// <summary>
        /// Calling record_frame will not cause the compositor to
        /// repaint, but it will wait instead for the first frame
        /// the compositor draws due to some other external event
        /// or internal change.
        /// Calling this request after calling record_frame will
        /// ask the compositor to redraw as soon at possible even
        /// if it wouldn't otherwise.
        /// If no frame was requested this request has no effect.
        /// </summary>
        public void Repaint()
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 2, __args);
        }

        public interface IEvents
        {
            /// <summary>
            /// This event will be sent immediately after creation of the
            /// lipstick_recorder object. The wl_buffers the client passes
            /// to the frame request must be big enough to store an image
            /// with the given width, height and format.
            /// If they are not the compositor will send the failed event.
            /// If this event is sent again later in the lifetime of the object
            /// the pending frames will be cancelled.
            /// 
            /// The format will be one of the values as defined in the
            /// wl_shm::format enum.
            /// </summary>
            void OnSetup(NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder eventSender, int @width, int @height, int @stride, int @format);

            /// <summary>
            /// The compositor will send this event after a frame was
            /// recorded, or in case an error happened. The client can
            /// call record_frame again to record the next frame.
            /// 
            /// 'time' is the time the compositor recorded that frame,
            /// in milliseconds, with an unspecified base.
            /// </summary>
            void OnFrame(NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder eventSender, NWayland.Protocols.Aurora.Wayland.WlBuffer @buffer, uint @time, int @transform);

            /// <summary>
            /// The value of the 'result' argument will be one of the
            /// values of the 'result' enum.
            /// </summary>
            void OnFailed(NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder eventSender, int @result, NWayland.Protocols.Aurora.Wayland.WlBuffer @buffer);

            /// <summary>
            /// The compositor will send this event if the client calls
            /// request_frame more than one time for the same compositor
            /// frame. The cancel event will be sent carrying the old
            /// buffer, and the frame will be recorded using the newest
            /// buffer.
            /// </summary>
            void OnCancelled(NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder eventSender, NWayland.Protocols.Aurora.Wayland.WlBuffer @buffer);
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
            switch (opcode)
            {
                case 0:
                    Events?.OnSetup(this, arguments[0].Int32, arguments[1].Int32, arguments[2].Int32, arguments[3].Int32);
                    break;
                case 1:
                    Events?.OnFrame(this, WlProxy.FromNative<NWayland.Protocols.Aurora.Wayland.WlBuffer>(arguments[0].IntPtr), arguments[1].UInt32, arguments[2].Int32);
                    break;
                case 2:
                    Events?.OnFailed(this, arguments[0].Int32, WlProxy.FromNative<NWayland.Protocols.Aurora.Wayland.WlBuffer>(arguments[1].IntPtr));
                    break;
                case 3:
                    Events?.OnCancelled(this, WlProxy.FromNative<NWayland.Protocols.Aurora.Wayland.WlBuffer>(arguments[0].IntPtr));
                    break;
            }
        }

        public enum ResultEnum
        {
            BadBuffer = 2
        }

        public enum TransformEnum
        {
            Normal = 1,
            YInverted = 2
        }

        private class ProxyFactory : IBindFactory<LipstickRecorder>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Aurora.LipstickRecorder.LipstickRecorder.WlInterface);
            }

            public LipstickRecorder Create(IntPtr handle, int version)
            {
                return new LipstickRecorder(handle, version);
            }
        }

        public static IBindFactory<LipstickRecorder> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "lipstick_recorder";
        public const int InterfaceVersion = 1;

        public LipstickRecorder(IntPtr handle, int version) : base(handle, version)
        {
        }
    }
}
