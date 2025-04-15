using System.Net;
using System.Text;
using Server.Services;

class Program
{
    static void Main()
    {
        string url = "http://localhost:17177/";
        HttpListener listener = new();
        listener.Prefixes.Add(url);
        listener.Prefixes.Add(url + "programs/");
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

                Console.WriteLine($"Получен POST-запрос от {context.Request.RemoteEndPoint}");
                Console.WriteLine($"Тело запроса:\n{json}");

                LoggerService.Info($"Получен POST-запрос от {context.Request.RemoteEndPoint}");
                LoggerService.Debug($"Тело запроса:\n{json}");

                response.StatusCode = 200;
                byte[] buffer = Encoding.UTF8.GetBytes("OK");
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
