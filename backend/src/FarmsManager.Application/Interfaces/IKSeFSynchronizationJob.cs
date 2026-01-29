namespace FarmsManager.Application.Interfaces;

/// <summary>
/// Interface dla job'a synchronizacji faktur z KSeF
/// </summary>
public interface IKSeFSynchronizationJob
{
    /// <summary>
    /// Wykonuje synchronizację faktur z KSeF
    /// </summary>
    /// <param name="isManual">Określa czy synchronizacja została wywołana manualnie (przez endpoint)</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task ExecuteSynchronizationAsync(bool isManual = false, CancellationToken cancellationToken = default);
}
