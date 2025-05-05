# win-app-tool
Консольное клиент-серверное приложение для автоматизированного сбора информации об установленном прикладном ПО на клиентских компьютерах и формирования отчётов в Excel.

## Структура проекта

- `Client/` — код клиентского приложения
- `Server/` — код серверной части
- `Models/` — общие модели данных
- `config/` — конфигурационные файлы (`allowed_clients.txt`, `mode.txt`)
- `output/` — лог-файлы и Excel-отчёты

## Команды запуска

### Клиентская часть

```bash
win-app-tool.exe --client [--url http://server:port] [--preview] [--send]
```

- `--client` — запуск клиента
- `--url` — адрес сервера (по умолчанию http://localhost:17177)
- `--preview` — выводит JSON-отчёт в консоль
- `--send` — отправляет JSON-отчёт на сервер

### Серверная часть

```bash
win-app-tool.exe --server [--strict]
```

- `--server` — запуск сервера
- `--strict` — включить строгий режим: принимать данные только от клиентов из `allowed_clients.txt`

## Пример использования

```bash
# Запуск сервера в режиме "accept-all"
win-app-tool.exe --server

# Запуск клиента и отправка данных на сервер
win-app-tool.exe --client --send
```

## Зависимости

- [.NET 8](https://dotnet.microsoft.com/)
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) — генерация Excel
- `System.Text.Json` — сериализация JSON
- `HttpListener` — встроенный HTTP-сервер



