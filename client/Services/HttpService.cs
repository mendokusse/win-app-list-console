using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Client.Services;

namespace Client.Services
{
    public class HttpService
    {
        private static readonly HttpClient client = new();

        public static void SendJson(string json, string serverUrl)
        {
            try
            {
                LoggerService.Info($"Начата отправка на сервер: {serverUrl}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // var response = await client.PostAsync(serverUrl, content);
                // LoggerService.Info($"Ответ сервера: {response.StatusCode}");

                LoggerService.Debug($"POST {serverUrl}");

                LoggerService.Info("Отправка завершена");
            }
            catch (Exception ex)
            {
                LoggerService.Error($" Ошибка при отправке: {ex.Message}");
            }
        }
    }
}
