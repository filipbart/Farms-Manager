namespace FarmsManager.Application.Models.KSeF;

/// <summary>
/// Konfiguracja synchronizacji KSeF
/// </summary>
public class KSeFSyncConfiguration
{
    /// <summary>
    /// Określa czy synchronizacja jest włączona
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// Interwał synchronizacji w godzinach
    /// </summary>
    public int IntervalHours { get; set; }
}
