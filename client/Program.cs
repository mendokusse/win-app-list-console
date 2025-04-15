using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Client.Services;
using Client;

class Program
{
    static void Main(string[] args)
    {
        LoggerService.Info("Запуск приложения.");

        string? serverUrl = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--url" && i + 1 < args.Length)
            {
                serverUrl = args[i + 1];
                LoggerService.Info($"Указан URL сервера: {serverUrl}");
            }
        }

        if (string.IsNullOrEmpty(serverUrl))
        {
            LoggerService.Error("Не указан адрес сервера.");
            return;
        }

        var apps = GetInstalledApps();
        string json = JsonService.SerializeToJson(apps);
        HttpService.SendJson(json, serverUrl);
    }

    static List<InstalledApp> GetInstalledApps()
    {
        var result = new List<InstalledApp>();

        LoggerService.Info("Сбор данных о приложениях начат.");

        string[] registryKeys = new string[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (string keyPath in registryKeys)
        {
            using (RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(keyPath))
            {
                if (baseKey == null) continue;

                foreach (var subkeyName in baseKey.GetSubKeyNames())
                {
                    using (var subkey = baseKey.OpenSubKey(subkeyName))
                    {
                        if (subkey == null) continue;

                        var name = subkey.GetValue("DisplayName") as string;
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        var app = new InstalledApp
                        {
                            DisplayName = name,
                            DisplayVersion = subkey.GetValue("DisplayVersion") as string,
                            Publisher = subkey.GetValue("Publisher") as string,
                            InstallDate = subkey.GetValue("InstallDate") as string
                        };

                        result.Add(app);
                    }
                }
            }
        }
        LoggerService.Info($"Найдено {result.Count} установленных программ.");

        return result;
    }
}

