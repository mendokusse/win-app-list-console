using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Client.Services
{
    public class JsonService
    {
        public static string SerializeToJson(List<InstalledApp> apps)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(apps, options);
        }
    }
}
