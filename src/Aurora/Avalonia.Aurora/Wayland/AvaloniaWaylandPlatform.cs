using Avalonia.Aurora.Wayland;
using Avalonia.Controls.Platform;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland
{
    internal class AvaloniaAuroraWaylandPlatform : IWindowingPlatform, IDisposable
    {
        public AvaloniaAuroraWaylandPlatform(AuroraWaylandPlatformOptions options)
        {
            Options = options;
            WlDisplay = WlDisplay.Connect();
            var registry = WlDisplay.GetRegistry();
            WlRegistryHandler = new WlRegistryHandler(registry);
            WlDisplay.Roundtrip();
            
            WlCompositor = WlRegistryHandler.BindRequiredInterface<WlCompositor>(WlInterface.WlCompositor.Name, WlInterface.WlCompositor.Version);
            WlSeat = WlRegistryHandler.BindRequiredInterface<WlSeat>(WlInterface.WlSeat.Name, WlInterface.WlSeat.Version);
            WlShm = WlRegistryHandler.BindRequiredInterface<WlShm>(WlInterface.WlShm.Name, WlInterface.WlShm.Version);
            WlDataDeviceManager = WlRegistryHandler.BindRequiredInterface<WlDataDeviceManager>(WlInterface.WlDataDeviceManager.Name, WlInterface.WlDataDeviceManager.Version);
            WlShell = WlRegistryHandler.BindRequiredInterface<WlShell>(WlInterface.WlShell.Name, WlInterface.WlShell.Version);
            //WpViewporter = WlRegistryHandler.Bind(WpViewporter.BindFactory, WpViewporter.InterfaceName, WpViewporter.InterfaceVersion);

            WlScreens = new WlScreens(this);
            WlInputDevice = new WlInputDevice(this);
            WlDataHandler = new WlDataHandler(this);
            WlRawEventGrouper = new WlRawEventGrouper();

            AvaloniaLocator.CurrentMutable
                .Bind<IWindowingPlatform>().ToConstant(this)
                .Bind<IDispatcherImpl>().ToConstant(new WlPlatformThreading(this))
                .Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60))
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control))
                .Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice())
                .Bind<ICursorFactory>().ToConstant(new WlCursorFactory(this))
                .Bind<IClipboard>().ToConstant(WlDataHandler)
                .Bind<IPlatformDragSource>().ToConstant(WlDataHandler)
                .Bind<IPlatformSettings>().ToSingleton<DBusPlatformSettings>()
                .Bind<IPlatformIconLoader>().ToConstant(new WlIconLoader())
                .Bind<IMountedVolumeInfoProvider>().ToConstant(new LinuxMountedVolumeInfoProvider());

            WlDisplay.Roundtrip();

            DBusHelper.TryInitialize();

            IPlatformGraphics? platformGraphics = null;

            if (options.UseGpu)
            {
                const int EGL_PLATFORM_WAYLAND_KHR = 0x31D8;
                platformGraphics = EglPlatformGraphics.TryCreate(() => new EglDisplay(new EglDisplayCreationOptions
                {
                    Egl = new EglInterface(),
                    PlatformType = EGL_PLATFORM_WAYLAND_KHR,
                    //PlatformType = 0x31D7,
                    PlatformDisplay = WlDisplay.RawPointer, // check
                    SupportsContextSharing = true,
                    SupportsMultipleContexts = true
                }));

                if (platformGraphics is not null)
                    AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(platformGraphics);
            }

            Compositor = new Compositor(platformGraphics);
        }

        internal AuroraWaylandPlatformOptions Options { get; }

        internal Compositor Compositor { get; }

        internal WlDisplay WlDisplay { get; }

        internal WlRegistryHandler WlRegistryHandler { get; }

        internal WlCompositor WlCompositor { get; }

        internal WlSeat WlSeat { get; }

        internal WlShm WlShm { get; }

        internal WlDataDeviceManager WlDataDeviceManager { get; }

        internal WlShell WlShell { get; }

        //internal WpViewporter? WpViewporter { get; }

        internal WlScreens WlScreens { get; }

        internal WlDataHandler WlDataHandler { get; }

        internal WlInputDevice WlInputDevice { get; }

        internal WlRawEventGrouper WlRawEventGrouper { get; }

        public IWindowImpl CreateWindow() => new WlToplevel(this);

        public IWindowImpl CreateEmbeddableWindow() => throw new NotSupportedException();

        public ITrayIconImpl? CreateTrayIcon()
        {
            var dbusTrayIcon = new DBusTrayIconImpl();
            if (!dbusTrayIcon.IsActive)
                return null;
            dbusTrayIcon.IconConverterDelegate = static impl => impl is WlIconData wlIconData ? wlIconData.Data : Array.Empty<uint>();
            return dbusTrayIcon;
        }

        //public void OnPing(XdgWmBase eventSender, uint serial) => XdgWmBase.Pong(serial);

        public void Dispose()
        {
            WlDataDeviceManager.Dispose();
            WlRawEventGrouper.Dispose();
            WlDataHandler.Dispose();
            WlInputDevice.Dispose();
            WlScreens.Dispose();
            WlSeat.Dispose();
            WlShm.Dispose();
            WlShell.Dispose();
            WlCompositor.Dispose();
            WlRegistryHandler.Dispose();
            WlDisplay.Dispose();
        }
    }
}

namespace Avalonia
{
    public static class AvaloniaWaylandPlatformExtensions
    {
        public static AppBuilder UseAuroraWayland(this AppBuilder builder) =>
            builder
                .UseStandardRuntimePlatformSubsystem()
                
                .UseWindowingSubsystem(static () =>
                {
                    var options = AvaloniaLocator.Current.GetService<AuroraWaylandPlatformOptions>() ?? new AuroraWaylandPlatformOptions();
                    var platform = new AvaloniaAuroraWaylandPlatform(options);
                    AvaloniaLocator.CurrentMutable.BindToSelf(platform);
                });
    }

    public class AuroraWaylandPlatformOptions
    {
        /// <summary>
        /// Determines whether to use GPU for rendering in your project. The default value is true.
        /// </summary>
        public bool UseGpu { get; set; } = true;

        /// <summary>
        /// The app ID identifies the general class of applications to which the surface belongs. <br/>
        /// The compositor can use this to group multiple surfaces together, or to determine how to launch a new application. <br/>
        /// As a best practice, it is suggested to select app ID's that match the basename of the application's .desktop file. For example, "org.freedesktop.FooViewer" where the .desktop file is "org.freedesktop.FooViewer.desktop".
        /// </summary>
        public string? AppId { get; set; }
    }
}
