using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Db
{
    public class Logger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();
        private readonly string _logPath;

        private Logger()
        {
            _logPath = Constants.Paths.LogPath;
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public void Log(LogLevel level, string message, Exception ex = null)
        {
            string logFile = Path.Combine(_logPath, $"Log_{DateTime.Now:yyyy-MM-dd}.txt");
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            if (ex != null)
            {
                logMessage += $"\r\nException: {ex.Message}\r\nStackTrace: {ex.StackTrace}";
            }

            try
            {
                using (StreamWriter writer = File.AppendText(logFile))
                {
                    writer.WriteLine(logMessage);
                    writer.WriteLine(new string('-', 80));
                }
            }
            catch
            {
                // Nếu không ghi được log, bỏ qua
            }
        }

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message, Exception ex = null) => Log(LogLevel.Error, message, ex);
    }
}
