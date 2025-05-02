using System;
using System.IO;

namespace Client.Services
{
    public static class LoggerService
    {
        private static string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static string logFile;

        static LoggerService()
        {
            Directory.CreateDirectory(logDirectory);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFile = Path.Combine(logDirectory, $"log_{timestamp}.txt");
        }

        public static void Info(string message) => Write("INFO", message);
        public static void Debug(string message) => Write("DEBUG", message);
        public static void Error(string message) => Write("ERROR", message);

        private static void Write(string level, string message)
        {
            string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            File.AppendAllText(logFile, logLine + Environment.NewLine);
        }
    }
}
