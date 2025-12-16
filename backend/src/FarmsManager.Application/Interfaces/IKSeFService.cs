using FarmsManager.Application.Common;
using FarmsManager.Application.Models.KSeF;

namespace FarmsManager.Application.Interfaces;

/// <summary>
/// Serwis do komunikacji z Krajowym Systemem e-Faktur (KSeF)
/// </summary>
public interface IKSeFService : IService
{
    /// <summary>
    /// Pobiera faktury z KSeF na podstawie kryteriów wyszukiwania
    /// </summary>
    /// <param name="request">Kryteria wyszukiwania faktur</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista faktur spełniających kryteria</returns>
    Task<KSeFInvoicesResponse> GetInvoicesAsync(KSeFInvoicesRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Pobiera wszystkie faktury z KSeF do synchronizacji
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista faktur do synchronizacji</returns>
    Task<List<KSeFInvoiceSyncItem>> GetInvoicesForSyncAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Pobiera szczegóły pojedynczej faktury z KSeF
    /// </summary>
    /// <param name="invoiceReferenceNumber">Numer referencyjny faktury w KSeF</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Szczegóły faktury</returns>
    Task<KSeFInvoiceDetails> GetInvoiceDetailsAsync(string invoiceReferenceNumber, CancellationToken cancellationToken);

    /// <summary>
    /// Pobiera XML faktury z KSeF
    /// </summary>
    /// <param name="invoiceReferenceNumber">Numer referencyjny faktury w KSeF</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>XML faktury</returns>
    Task<string> GetInvoiceXmlAsync(string invoiceReferenceNumber, CancellationToken cancellationToken);

    Task<string> SendTestInvoiceAsync(string fileContent, CancellationToken cancellationToken);
}