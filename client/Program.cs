using System;
using System.Collections.Generic;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        var apps = GetInstalledApps();
    }

    static List<InstalledApp> GetInstalledApps()
    {
        var result = new List<InstalledApp>();

        // Основные ветки, где хранятся установленные программы
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
                        if (string.IsNullOrWhiteSpace(name)) continue; // пропускаем системное

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

        return result;
    }
}

class InstalledApp
{
    public string? DisplayName { get; set; }
    public string? DisplayVersion { get; set; }
    public string? Publisher { get; set; }
    public string? InstallDate { get; set; }
}
