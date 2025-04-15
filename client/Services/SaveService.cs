using System;
using System.IO;

namespace Client.Services
{
    public static class SaveService
    {
        public static void SaveToFile(string json)
        {
            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            Directory.CreateDirectory(outputDir);

            string fileName = $"installed_programs_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string path = Path.Combine(outputDir, fileName);

            File.WriteAllText(path, json);

            LoggerService.Info($"JSON сохранён в файл: {path}");
        }
    }
}
