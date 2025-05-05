using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Client.Services;
using Models;

namespace Client
{
    public static class ProgramClient
    {
        public static void Run(string[] args)
        {
            LoggerService.Info("Запуск приложения");

            bool doPreview = false;
            bool doSend = false;
            bool doSave = false;
            string? serverUrl = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--preview":
                        doPreview = true;
                        break;
                    case "--send":
                        doSend = true;
                        break;
                    case "--save":
                        doSave = true;
                        break;
                    case "--url":
                        if (i + 1 < args.Length)
                        {
                            serverUrl = args[i + 1];
                            LoggerService.Info($"Указан URL сервера: {serverUrl}");
                            i++;
                        }
                        break;
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return;
                }
            }

            if (doSend && string.IsNullOrEmpty(serverUrl))
            {
                LoggerService.Error("Указан флаг --send, но не задан --url.");
                return;
            }

            var apps = GetInstalledApps();
            var clientData = new ClientData
            {
                Username = Environment.UserName,
                Hostname = Environment.MachineName,
                Programs = apps
            };

            string json = JsonService.SerializeToJson(clientData);

            if (doPreview)
            {
                Console.WriteLine("Программа покажет JSON ниже:");
                Console.WriteLine(json);
            }

            if (doSave)
            {
                SaveService.SaveToFile(json);
            }

            if (doSend)
            {
                HttpService.SendJson(json, serverUrl!);
            }

            Console.ReadKey();
            LoggerService.Info("Программа завершила работу.");
        }

        static void ShowHelp()
        {
            Console.WriteLine(@"
        Использование:
        win-app-list-client -- [опции]

        Опции:
        --preview              Показать JSON в консоли
        --save                 Сохранить JSON в файл
        --send                 Отправить JSON на сервер (нужен --url)
        --url <адрес>          Адрес сервера
        --help, -h             Показать эту справку
        ");
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
}

