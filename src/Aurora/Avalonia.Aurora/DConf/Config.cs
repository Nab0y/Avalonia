using System.Globalization;

namespace Avalonia.Aurora.DConf;

public sealed class Config : IDisposable
{
    private Client _dconfClient;
    
    public Config()
    {
        _dconfClient = new Client();
    }
    
    public void Dispose()
    {
        _dconfClient.Dispose();
    }
    
    public double GetThemePixelRatio()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/theme_pixel_ratio");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
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

    public string GetFontFamily()
    {
        return GetConfigValue("/desktop/sailfish/silica/font_family");
    }
    
    public string GetFontFamilyHeading()
    {
        return GetConfigValue("/desktop/sailfish/silica/font_family_heading");
    }
    
    public double GetFontSizeExtraLarge()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_extra_large");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetFontSizeExtraSmall()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_extra_small");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public double GetFontSizeHuge()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_huge");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetFontSizeLarge()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_large");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetFontSizeMedium()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_medium");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
    
    public double GetFontSizeTiny()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/font_size_tiny");
        return double.Parse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public bool GetIsAutoScaleValues()
    {
        var strValue = GetConfigValue("/desktop/sailfish/silica/auto_scale_values");
        return bool.Parse(strValue);
    }
    
    public string GetConfigValue(string key)
    {
        var valuePtr = LibDconf.Read(_dconfClient.Handler, key);
        return LibGObject.GVariantPrint(valuePtr, false);
    }
}
