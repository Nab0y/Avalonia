namespace Avalonia.Aurora.DConf;

internal sealed class Client
{
    private static volatile Client? _instance;
    private static object _syncObject = new();
    public static Client Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_syncObject)
                {
                    if (_instance == null)
                    {
                        _instance = new Client();
                    }
                }
            }

            return _instance;
        }
    }
    
    public IntPtr Handler { get; private set; }
    
    private Client()
    {
        Handler = LibDconf.NewClient();
    }
}
