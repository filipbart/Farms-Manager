namespace FarmsManager.HostBuilder.Configuration.Options;

public class SerilogOptions
{
    public FileOptions File { get; init; }

    public record FileOptions
    {
        public bool Enabled { get; init; }
        public string Path { get; init; }
    }
}