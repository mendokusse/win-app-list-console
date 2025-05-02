using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Server;
using Models;

namespace Server.Services
{
    public static class ExcelService
    {
        private static readonly string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
        private static readonly string excelPath = Path.Combine(outputDir, "installed_programs.xlsx");

        static ExcelService()
        {
            Directory.CreateDirectory(outputDir);
        }

        public static void SavePrograms(List<InstalledApp> programs, string clientId)
        {
            clientId = SanitizeSheetName(clientId);
            using var workbook = File.Exists(excelPath)
                ? new XLWorkbook(excelPath)
                : new XLWorkbook();

            if (workbook.Worksheets.Contains(clientId))
                workbook.Worksheet(clientId).Delete();

            var worksheet = workbook.Worksheets.Add(clientId);

            worksheet.Cell(1, 1).Value = "Название";
            worksheet.Cell(1, 2).Value = "Версия";
            worksheet.Cell(1, 3).Value = "Производитель";
            worksheet.Cell(1, 4).Value = "Дата установки";

            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            for (int i = 0; i < programs.Count; i++)
            {
                var app = programs[i];
                int row = i + 2;

                worksheet.Cell(row, 1).Value = app.DisplayName;
                worksheet.Cell(row, 2).Value = app.DisplayVersion;
                worksheet.Cell(row, 3).Value = app.Publisher;
                worksheet.Cell(row, 4).Value = app.InstallDate;

                if (DateTime.TryParse(app.InstallDate, out DateTime parsedDate))
                {
                    worksheet.Cell(row, 4).Value = parsedDate;
                    worksheet.Cell(row, 4).Style.DateFormat.Format = "dd.MM.yyyy";
                }
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(excelPath);
            LoggerService.Info($"Данные клиента '{clientId}' сохранены в Excel.");
        }

        public static string SanitizeSheetName(string name)
        {
            var invalidChars = new[] { ':', '\\', '/', '*', '?', '[', ']' };
            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '_');
            }

            return name.Length > 31 ? name.Substring(0, 31) : name;
        }
    }
}
