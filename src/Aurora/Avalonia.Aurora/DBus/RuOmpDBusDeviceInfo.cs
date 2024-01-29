using Tmds.DBus.Protocol;

namespace Avalonia.Aurora.DBus;

internal class RuOmpDBusDeviceInfo
{
    private const string Interface = "ru.omp.deviceinfo.Features";
    private const string Destination = "ru.omp.deviceinfo";
    private const string Path = "/ru/omp/deviceinfo/Features";
    private readonly Connection _connection;

    public RuOmpDBusDeviceInfo(Connection connection)
    {
        _connection = connection;
    }

    public Task<uint> GetBatteryChargePercentageAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getBatteryChargePercentage"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        });
    }

    public Task<string> GetCpuModelAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getBatteryChargePercentage"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        });
    }
    
    public Task<string> GetDeviceModelAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getDeviceModel"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        });
    }
    
    public Task<double> GetFrontalCameraResolutionAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getFrontalCameraResolution"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadDouble();
        });
    }
    
    public Task<double> GetMainCameraResolutionAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getMainCameraResolution"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadDouble();
        });
    }
    
    public Task<uint> GetMaxCpuClockSpeedAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getMaxCpuClockSpeed"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        });
    }
    
    // public Task<object[]> GetMaxCpuCoresClockSpeedAsync()
    // {
    //     return _connection.CallMethodAsync(CreateMessage("getMaxCpuCoresClockSpeed"), (message, state) =>
    //     {
    //         var reader = message.GetBodyReader();
    //         return reader.ReadArray<object>();
    //     });
    // }
    
    public Task<uint> GetNumberCpuCoresAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getNumberCpuCores"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        });
    }
    
    public Task<string> GetOsVersionAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getOsVersion"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        });
    }
    
    public Task<ulong> GetRamFreeSizeAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getRamFreeSize"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt64();
        });
    }
    
    public Task<ulong> GetRamTotalSizeAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getRamTotalSize"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt64();
        });
    }

    public Task<PixelSize> GetScreenResolutionAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getScreenResolution"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            var strResolution = reader.ReadString();
            var values = strResolution.Split('x');
            if (values.Length != 2)
            {
                return PixelSize.Empty;
            }

            if (!int.TryParse(values[0], out var width))
            {
                return PixelSize.Empty;
            }

            if (!int.TryParse(values[1], out var height))
            {
                return PixelSize.Empty;
            }

            return new PixelSize(width, height);
        });
    }
    
    public Task<string> GetSerialNumberAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("getSerialNumber"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        });
    }
    
    public Task<bool> HasBluetoothAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("hasBluetooth"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        });
    }
    
    public Task<bool> HasGNSSAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("hasGNSS"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        });
    }
    
    public Task<bool> HasNFCAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("hasNFC"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        });
    }
    
    public Task<bool> HasWlanAsync()
    {
        return _connection.CallMethodAsync(CreateMessage("hasWlan"), (message, state) =>
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        });
    }
    
    private MessageBuffer CreateMessage(string memberName)
    {
        var writer = _connection.GetMessageWriter();
        writer.WriteMethodCallHeader(Destination, Path, Interface, memberName);
        var message = writer.CreateMessage();
        writer.Dispose();
        return message;
    }
}
