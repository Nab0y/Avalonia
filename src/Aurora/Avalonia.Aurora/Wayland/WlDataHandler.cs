using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using WaylandSharp;

namespace Avalonia.Aurora.Wayland;

internal class WlDataHandler : IClipboard, IPlatformDragSource, IDisposable
{
    private readonly AvaloniaAuroraWaylandPlatform _platform;
    private readonly WlDataDevice _wlDataDevice;
    //private readonly WlDataDeviceHandler _wlDataDeviceHandler;
    
    private uint _enterSerial;
    private Point _position;
    private WlDataObject? _currentOffer;
    private WlDataObject? _dndOffer;

    private WlWindow? _dragWindow;

    private WlDataSourceHandler? _currentDataSourceHandler;

    public WlDataHandler(AvaloniaAuroraWaylandPlatform platform)
    {
        _platform = platform;
        _wlDataDevice = platform.WlDataDeviceManager.GetDataDevice(platform.WlSeat);
        _wlDataDevice.Enter += OnEnter;
        _wlDataDevice.Leave += OnLeave;
        _wlDataDevice.Drop += OnDrop;
        _wlDataDevice.Motion += OnMotion;
        _wlDataDevice.Selection += OnSelection;
        _wlDataDevice.DataOffer += OnDataOffer;
    }

    public void Dispose()
    {
        _wlDataDevice.Enter -= OnEnter;
        _wlDataDevice.Leave -= OnLeave;
        _wlDataDevice.Drop -= OnDrop;
        _wlDataDevice.Motion -= OnMotion;
        _wlDataDevice.Selection -= OnSelection;
        _wlDataDevice.DataOffer -= OnDataOffer;
        _wlDataDevice.Dispose();
    }

    private void OnDataOffer(object? sender, WlDataDevice.DataOfferEventArgs e)
    {
        _currentOffer = new WlDataObject(_platform, e.Id);
    }

    private void OnSelection(object? sender, WlDataDevice.SelectionEventArgs e)
    {
        DisposeSelection();
        if (_currentOffer?.WlDataOffer != e.Id)
        {
            return;
        }

        SelectionOffer = _currentOffer;
        _currentOffer = null;
    }

    private void OnMotion(object? sender, WlDataDevice.MotionEventArgs e)
    {
        if (_dragWindow?.InputRoot is null || _dndOffer is null)
            return;
        _position = new Point((int)e.X, (int)e.Y);
        var dragDropDevice = AvaloniaLocator.Current.GetRequiredService<IDragDropDevice>();
        var modifiers = _platform.WlInputDevice.RawInputModifiers;
        var args = new RawDragEvent(dragDropDevice, RawDragEventType.DragOver, _dragWindow.InputRoot, _position, _dndOffer, _dndOffer.OfferedDragDropEffects, modifiers);
        _dragWindow.Input?.Invoke(args);
        Accept(args);
    }

    private void OnDrop(object? sender, WlDataDevice.DropEventArgs e)
    {
        if (_dragWindow?.InputRoot is null || _dndOffer is null)
            return;
        var dragDropDevice = AvaloniaLocator.Current.GetRequiredService<IDragDropDevice>();
        var modifiers = _platform.WlInputDevice.RawInputModifiers;
        var args = new RawDragEvent(dragDropDevice, RawDragEventType.Drop, _dragWindow.InputRoot, _position, _dndOffer, _dndOffer.MatchedDragDropEffects, modifiers);
        _dragWindow.Input?.Invoke(args);
        if (args.Effects != DragDropEffects.None)
        {
            //_dndOffer?.WlDataOffer.Finish(); // check
        }
        else
        {
            DisposeCurrentDnD();
        }
    }

    private void OnLeave(object? sender, WlDataDevice.LeaveEventArgs e)
    {
        DisposeCurrentDnD();
    }

    private void OnEnter(object? sender, WlDataDevice.EnterEventArgs e)
    {
        DisposeCurrentDnD();
        if (_currentOffer is null || _currentOffer.WlDataOffer != e.Id)
            return;
        _enterSerial = e.Serial;
        _dndOffer = _currentOffer;
        _currentOffer = null;
        _dragWindow = _platform.WlScreens.WindowFromSurface(e.Surface);
        if (_dragWindow?.InputRoot is null)
            return;
        _position = new Point((int)e.X, (int)e.Y);
        var dragDropDevice = AvaloniaLocator.Current.GetRequiredService<IDragDropDevice>();
        var modifiers = _platform.WlInputDevice.RawInputModifiers;
        var args = new RawDragEvent(dragDropDevice, RawDragEventType.DragEnter, _dragWindow.InputRoot, _position, _dndOffer, _dndOffer.OfferedDragDropEffects, modifiers);
        _dragWindow.Input?.Invoke(args);
        Accept(args);
    }
    
    internal WlDataObject? SelectionOffer { get; private set; }
    
    private void Accept(RawDragEvent args)
    {
        if (_dndOffer == null)
        {
            return;
        }
        var preferredAction = GetPreferredEffect(args.Effects, _platform.WlInputDevice.RawInputModifiers);
        //_dndOffer!.WlDataOffer.SetActions((WlDataDeviceManager.DndActionEnum)args.Effects, (WlDataDeviceManager.DndActionEnum)preferredAction);
        if (_dndOffer.MimeTypes.Count > 0 && args.Effects != DragDropEffects.None)
            _dndOffer.WlDataOffer.Accept(_enterSerial, _dndOffer.MimeTypes[0]);
        else
            _dndOffer.WlDataOffer.Accept(_enterSerial, string.Empty); // check
    }

    private void DisposeSelection()
    {
        SelectionOffer?.Dispose();
        SelectionOffer = null;
    }

    private void DisposeCurrentDnD()
    {
        _dndOffer?.Dispose();
        _dndOffer = null;
    }

    private static DragDropEffects GetPreferredEffect(DragDropEffects effect, RawInputModifiers modifiers)
    {
        if (effect is DragDropEffects.Copy or DragDropEffects.Move or DragDropEffects.None)
            return effect; // No need to check for the modifiers.
        if (effect.HasAllFlags(DragDropEffects.Copy) && modifiers.HasAllFlags(RawInputModifiers.Control))
            return DragDropEffects.Copy;
        return DragDropEffects.Move;
    }

    public Task<string?> GetTextAsync() => Task.FromResult(SelectionOffer?.GetText() ?? null);

    public Task SetTextAsync(string? text)
    {
        if (text is null)
            return ClearAsync();
        var data = new DataObject();
        data.Set(DataFormats.Text, text);
        return SetDataObjectAsync(data);
    }

    public Task ClearAsync()
    {
        if (_platform.WlInputDevice.KeyboardHandler is null)
            return Task.CompletedTask;
        //_wlDataDevice.SetSelection(null, _platform.WlInputDevice.KeyboardHandler.KeyboardEnterSerial);
        _wlDataDevice.Release();
        
        return Task.CompletedTask;
    }

    public Task SetDataObjectAsync(IDataObject data)
    {
        if (_platform.WlInputDevice.KeyboardHandler is null)
            return Task.CompletedTask;
        var dataSource = _platform.WlDataDeviceManager.CreateDataSource();
        _currentDataSourceHandler = new WlDataSourceHandler(_platform, dataSource, data);
        _wlDataDevice.SetSelection(dataSource, _platform.WlInputDevice.KeyboardHandler.KeyboardEnterSerial);
        return Task.CompletedTask;
    }

    public Task<string[]> GetFormatsAsync() =>
        SelectionOffer is null
            ? Task.FromResult(Array.Empty<string>())
            : Task.FromResult(SelectionOffer.GetDataFormats().ToArray());

    public Task<object?> GetDataAsync(string format) => Task.FromResult(SelectionOffer?.Get(format));

    public Task<DragDropEffects> DoDragDrop(PointerEventArgs triggerEvent, IDataObject data, DragDropEffects allowedEffects)
    {
        var toplevel = (triggerEvent.Source as Visual)?.VisualRoot as TopLevel;
        if (toplevel?.PlatformImpl is not WlWindow wlWindow)
            return Task.FromResult(DragDropEffects.None);
        triggerEvent.Pointer.Capture(null);
        var dataSource = _platform.WlDataDeviceManager.CreateDataSource();
        _currentDataSourceHandler = new WlDataSourceHandler(_platform, dataSource, data, allowedEffects);
        _wlDataDevice.StartDrag(dataSource, wlWindow.WlSurface, null, _platform.WlInputDevice.Serial);
        return _currentDataSourceHandler.DnD;
    }


    private sealed class WlDataSourceHandler
    {
        private readonly AvaloniaAuroraWaylandPlatform _platform;
        private readonly WlDataSource _wlDataSource;
        private readonly IDataObject _dataObject;
        private readonly TaskCompletionSource<DragDropEffects> _dnd;

        
        //private WlDataDeviceManager.DndActionEnum _lastDnDAction;

        public WlDataSourceHandler(AvaloniaAuroraWaylandPlatform platform, WlDataSource wlDataSource, IDataObject dataObject)
        {
            _platform = platform;
            _wlDataSource = wlDataSource;
            _wlDataSource.Cancelled += OnCancelled;
            _wlDataSource.Send += OnSend;
            _wlDataSource.Target += OnTarget;
            
            _dataObject = dataObject;
            _dnd = new TaskCompletionSource<DragDropEffects>();
            foreach (var format in dataObject.GetDataFormats())
            {
                switch (format)
                {
                    case nameof(DataFormats.Text):
                        _wlDataSource.Offer(MimeTypes.Text);
                        _wlDataSource.Offer(MimeTypes.TextUtf8);
                        break;
                    case nameof(DataFormats.Files):
                        _wlDataSource.Offer(MimeTypes.UriList);
                        break;
                }
            }
        }

        private void OnTarget(object? sender, WlDataSource.TargetEventArgs e)
        {
            throw new NotImplementedException();
        }

        private unsafe void OnSend(object? sender, WlDataSource.SendEventArgs e)
        {
            var content = e.MimeType switch
            {
                MimeTypes.Text or MimeTypes.TextUtf8 when _dataObject.GetText() is { } text => ToBytes(text),
                MimeTypes.UriList when _dataObject.GetFiles() is { } uris => ToBytes(uris),
                _ => null
            };

            if (content is not null)
                fixed (byte* ptr = content)
                    LibC.write(e.Fd, (IntPtr)ptr, content.Length);

            LibC.close(e.Fd);
        }

        private void OnCancelled(object? sender, WlDataSource.CancelledEventArgs e)
        {
            _wlDataSource.Cancelled -= OnCancelled;
            _wlDataSource.Send -= OnSend;
            _wlDataSource.Target -= OnTarget;
            
            _wlDataSource.Dispose();
            _dnd.TrySetResult(DragDropEffects.None);
        }
        
        public WlDataSourceHandler(AvaloniaAuroraWaylandPlatform platform, WlDataSource wlDataSource, IDataObject dataObject, DragDropEffects allowedEffects) : this(platform, wlDataSource, dataObject)
        {
            // var actions = WlDataDeviceManager.DndActionEnum.None;
            // if (allowedEffects.HasAllFlags(DragDropEffects.Copy))
            //     actions |= WlDataDeviceManager.DndActionEnum.Copy;
            // if (allowedEffects.HasAllFlags(DragDropEffects.Move))
            //     actions |= WlDataDeviceManager.DndActionEnum.Move;
            // wlDataSource.SetActions(actions);
        }

        internal Task<DragDropEffects> DnD => _dnd.Task;

       
        // public void OnDndDropPerformed(WlDataSource eventSender) => _dnd.TrySetResult((DragDropEffects)_lastDnDAction);
        //
        // public void OnDndFinished(WlDataSource eventSender) => _wlDataSource.Dispose();
        //
        // public void OnAction(WlDataSource eventSender, WlDataDeviceManager.DndActionEnum dndAction)
        // {
        //     _lastDnDAction = dndAction;
        //     var cursorFactory = AvaloniaLocator.Current.GetRequiredService<ICursorFactory>();
        //     ICursorImpl? cursor = null;
        //     if (dndAction.HasAllFlags(WlDataDeviceManager.DndActionEnum.Copy))
        //         cursor = cursorFactory.GetCursor(StandardCursorType.DragCopy);
        //     else if (dndAction.HasAllFlags(WlDataDeviceManager.DndActionEnum.Move))
        //         cursor = cursorFactory.GetCursor(StandardCursorType.DragMove);
        //     _platform.WlInputDevice.PointerHandler?.SetCursor(cursor as WlCursor);
        // }

        private static byte[] ToBytes(string text) => Encoding.UTF8.GetBytes(text);

        private static byte[] ToBytes(IEnumerable<IStorageItem> storageItems) => storageItems.SelectMany(static x => Encoding.UTF8.GetBytes(x.Path.LocalPath).Append((byte)'\n')).ToArray();
    }
}
