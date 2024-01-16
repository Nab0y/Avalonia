using Avalonia;

namespace MobileSandbox.Aurora;

class Program
{   
    [STAThread]
    static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UseWayland()
            .UseSkia()
            .LogToTrace();
    }
}
