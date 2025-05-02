using System.Text.Json.Serialization;

namespace Client.Models {
    public class InstalledApp
    {
        public string? DisplayName { get; set; }
        public string? DisplayVersion { get; set; }
        public string? Publisher { get; set; }

        [JsonIgnore]
        public string? InstallDate { get; set; }

        [JsonPropertyName("InstallDate")]
        public string? FormattedInstallDate
        {
            get
            {
                if (DateTime.TryParseExact(InstallDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    return parsedDate.ToString("dd.MM.yyyy");
                }
                return null;
            }
        }
    }
}