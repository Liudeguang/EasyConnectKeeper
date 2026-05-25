using System;
using System.IO;
using System.Text;

namespace EasyConnectKeeper
{
    public class LogService
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private Action<string>? _onLog;

        public LogService(Action<string>? onLog = null)
        {
            _onLog = onLog;
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public void Info(string message)
        {
            Log("INFO", message);
        }

        public void Error(string message, Exception? ex = null)
        {
            string fullMessage = ex != null ? $"{message} | Exception: {ex.Message}" : message;
            Log("ERROR", fullMessage);
        }

        private void Log(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedMessage = $"[{timestamp}] [{level}] {message}";

            // Write to file
            try
            {
                string logFile = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
                File.AppendAllText(logFile, formattedMessage + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // Ignore file logging errors to prevent app crash
            }

            // Invoke callback for UI update
            _onLog?.Invoke(formattedMessage);
        }

        public void SetCallback(Action<string> onLog)
        {
            _onLog = onLog;
        }
    }
}
