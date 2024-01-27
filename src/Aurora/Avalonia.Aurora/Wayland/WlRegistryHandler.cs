using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlRegistryHandler : IDisposable
{
    private readonly WlRegistry _registry;
    private readonly Dictionary<uint, GlobalInfo> _globals = new();

    public WlRegistryHandler(WlRegistry registry)
    {
        _registry = registry;
        _registry.Global += RegistryOnGlobal;
        _registry.GlobalRemove += RegistryOnGlobalRemove;
    }

    private void RegistryOnGlobal(object? sender, WlRegistry.GlobalEventArgs e)
    {
        var global = new GlobalInfo(e.Name, e.Interface, (int)e.Version);
        _globals[e.Name] = global;
        _globalAdded?.Invoke(global);
    }
    
    private void RegistryOnGlobalRemove(object? sender, WlRegistry.GlobalRemoveEventArgs e)
    {
        if (!_globals.Remove(e.Name, out var glob))
        {
            return;
        }

        GlobalRemoved?.Invoke(glob);
    }

    private Action<GlobalInfo>? _globalAdded;
    public event Action<GlobalInfo>? GlobalAdded
    {
        add
        {
            _globalAdded += value;
            foreach (var global in _globals.Values)
                value?.Invoke(global);
        }
        remove => _globalAdded -= value;
    }

    public event Action<GlobalInfo>? GlobalRemoved;

    
    // public T BindRequiredInterface<T>(IBindFactory<T> factory, string @interface, int version) where T : WlProxy =>
    //     Bind(factory, @interface, version) ?? throw new WaylandPlatformException($"Failed to bind required interface {@interface}.");
    //
    // public T BindRequiredInterface<T>(IBindFactory<T> factory, int version, GlobalInfo global) where T : WlProxy =>
    //     Bind(factory, version, global) ?? throw new WaylandPlatformException($"Failed to bind required interface {global.Interface}.");

    // public T? Bind<T>(IBindFactory<T> factory, string @interface, int version) where T : WlProxy
    // {
    //     var global = _globals.Values.FirstOrDefault(g => g.Interface == @interface);
    //     return global is null ? null : Bind(factory, version, global);
    // }
    //
    // public T? Bind<T>(IBindFactory<T> factory, int version, GlobalInfo global) where T : WlProxy
    // {
    //     var requestVersion = Math.Min(version, global.Version);
    //     return _registry.Bind(global.Name, factory, requestVersion);
    // }

    public T BindRequiredInterface<T>(string @interface, int version) where T : WlClientObject
    {
        var global = _globals.Values.FirstOrDefault(g => g.Interface == @interface);
        if (global == null)
        {
            throw new WaylandPlatformException($"Failed to bind required interface {@interface}.");
        }

        return Bind<T>(version, global);
    }

    public T BindRequiredInterface<T>(int version, GlobalInfo global) where T : WlClientObject =>
        Bind<T>(version, global) ?? throw new WaylandPlatformException($"Failed to bind required interface {global.Interface}.");
    
    public T? Bint<T>(string @interface, int version) where T : WlClientObject
    {
        var global = _globals.Values.FirstOrDefault(g => g.Interface == @interface);
        return global is null ? null : Bind<T>(version, global);
    }

    public T Bind<T>(int version, GlobalInfo global) where T : WlClientObject
    {
        var requestVersion = Math.Min(version, global.Version);
        return _registry.Bind<T>(global.Name, global.Interface, (uint)requestVersion);
    }

    public void Dispose()
    {
        _registry.Global -= RegistryOnGlobal;
        _registry.GlobalRemove -= RegistryOnGlobalRemove;
        _registry.Dispose();
    }

    public sealed class GlobalInfo
    {
        internal GlobalInfo(uint name, string @interface, int version)
        {
            Name = name;
            Interface = @interface;
            Version = version;
        }

        public uint Name { get; }

        public string Interface { get; }

        public int Version { get; }

        public override string ToString() => $"{Interface} version {Version} at {Name}";
    }
}
