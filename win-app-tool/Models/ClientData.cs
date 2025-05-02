namespace Models
{
    public class ClientData
    {
        public string? Username { get; set; }
        public string? Hostname { get; set; }
        public List<InstalledApp> Programs { get; set; } = new();
    }
}