using System.Globalization;

namespace Avalonia.Aurora.DConf;

public sealed class Config
{
    public double GetPhysDpi()
    {
        var strValue = GetConfigValue("/lipstick/screen/primary/physicalDotsPerInch");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetPhysDpiX()
    {
        var strValue = GetConfigValue("/lipstick/screen/primary/physicalDotsPerInchX");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetPhysDpiY()
    {
        var strValue = GetConfigValue("/lipstick/screen/primary/physicalDotsPerInchY");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public PixelSize GetScreenResolution()
    {
        return new PixelSize(GetScreenWidth(), GetScreenHeight());
    }

    public int GetScreenWidth()
    {
        var strValue = GetConfigValue("/lipstick/screen/primary/width");
        return int.Parse(strValue);
    }
    
    public int GetScreenHeight()
    {
        var strValue = GetConfigValue("/lipstick/screen/primary/height");
        return int.Parse(strValue);
    }

    public string GetConfigValue(string key)
    {
        var valuePtr = LibDconf.Read(Client.Instance.Handler, key);
        return LibGObject.GVariantPrint(valuePtr, false);
    }
}
