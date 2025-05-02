using System.Net;
using System.Text;
using Server.Services;
using System.Text.Json;
using Models;

namespace Server
{
    public static class ProgramServer
    {
        public static void Run(string[] args)
        {
            string url = "http://localhost:17177/";

            bool strictMode = false;

            foreach (var arg in args)
            {
                if (arg == "--strict")
                {
                    strictMode = true;
                    LoggerService.Info("Включён строгий режим проверки клиентов (strict mode).");
                }

                if (arg == "--help" || arg == "-h")
                {
                    Console.WriteLine(@"
                    win-app-list-server -- [опции]

                    Опции:
                    --strict        Принимать только клиентов из config/allowed_clients.txt
                    --help, -h      Показать эту справку

                    По умолчанию сервер принимает запросы от всех клиентов (accept-all).
                    ");
                    return;
                }
            }

            var configDir = "config";
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            var allowedClientsPath = Path.Combine(configDir, "allowed_clients.txt");
            var allowedClients = File.Exists(allowedClientsPath)
                ? new HashSet<string>(File.ReadAllLines(allowedClientsPath))
                : new HashSet<string>();
            HttpListener listener = new();
            listener.Prefixes.Add(url);
            listener.Prefixes.Add(url + "programs/");
            listener.Prefixes.Add("http://*:17177/programs/");
            listener.Prefixes.Add("http://*:17177/");
            listener.Start();

            Console.WriteLine($"HTTP-сервер запущен по адресу: {url}");
            LoggerService.Info($"HTTP-сервер запущен по адресу: {url}");

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                LoggerService.Debug($"Метод: {request.HttpMethod}, Путь: {request.Url.AbsolutePath}");
                if (request.HttpMethod == "POST" && request.Url?.AbsolutePath.TrimEnd('/') == "/programs")
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    var json = reader.ReadToEnd();
                    var clientData = JsonSerializer.Deserialize<ClientData>(json);

                    string clientId = ExcelService.SanitizeSheetName($"{clientData?.Hostname}_{clientData?.Username}");

                    if (strictMode && !allowedClients.Contains(clientId))
                    {
                        LoggerService.Error($"Клиент '{clientId}' не разрешён (strict mode).");

                        string unknownClientsPath = Path.Combine(configDir, "unknown_clients.txt");
                        File.AppendAllText(unknownClientsPath, clientId + Environment.NewLine);

                        response.StatusCode = 403;
                        response.Close();
                        continue;
                    }

                    ExcelService.SavePrograms(clientData?.Programs ?? new(), clientId);

                    Console.WriteLine($"Получен POST-запрос от {context.Request.RemoteEndPoint}");

                    LoggerService.Info($"Получен POST-запрос от {context.Request.RemoteEndPoint}");
                    LoggerService.Debug($"Тело запроса:\n{json}");

                    byte[] buffer = Encoding.UTF8.GetBytes("OK");

                    response.StatusCode = 200;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = 404;
                }

                response.Close();
            }
        }
    }
}
