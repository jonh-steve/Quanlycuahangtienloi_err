using System.Configuration;

public class AppSettings
{
    private static AppSettings _instance;
    private static readonly object _lock = new object();

    private AppSettings() { }

    public static AppSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppSettings();
                    }
                }
            }
            return _instance;
        }
    }

    public string ConnectionString => ConfigurationManager.ConnectionStrings["QuanLyCuaHangTienLoi"].ConnectionString;
    public string AppName => ConfigurationManager.AppSettings["AppName"];
    public string Version => ConfigurationManager.AppSettings["Version"];
    public string Theme => ConfigurationManager.AppSettings["Theme"];
    public string LogLevel => ConfigurationManager.AppSettings["LogLevel"];
    public string LogPath => ConfigurationManager.AppSettings["LogPath"];
    public string BackupPath => ConfigurationManager.AppSettings["BackupPath"];
}