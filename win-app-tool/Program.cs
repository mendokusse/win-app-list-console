using Client;
using Server;

class Program
{
    static void Main(string[] args)
    {
        if (args.Contains("--client"))
        {
            ProgramClient.Run(args);
        }
        else if (args.Contains("--server"))
        {
            ProgramServer.Run(args);
        }
        else
        {
            Console.WriteLine(@"
            Использование: dotnet run -- [опции]

            --client        Запустить клиент
            --server        Запустить сервер
            --strict        Только для сервера: проверка разрешённых клиентов
            --help, -h      Показать справку
            ");
        }
    }
}
