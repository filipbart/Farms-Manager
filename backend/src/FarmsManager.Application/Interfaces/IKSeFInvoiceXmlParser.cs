using FarmsManager.Application.Common;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Domain.Aggregates.AccountingAggregate.XmlModels;

namespace FarmsManager.Application.Interfaces;

/// <summary>
/// Interfejs parsera XML faktur KSeF
/// </summary>
public interface IKSeFInvoiceXmlParser : IService
{
    /// <summary>
    /// Parsuje XML faktury KSeF do modelu
    /// </summary>
    KSeFInvoiceXml ParseInvoiceXml(string xml);

    /// <summary>
    /// Konwertuje sparsowany XML na model szczegółów faktury
    /// </summary>
    KSeFInvoiceDetails ToInvoiceDetails(KSeFInvoiceXml invoice, string ksefNumber);

    /// <summary>
    /// Wyciąga podstawowe dane z XML bez pełnego parsowania
    /// </summary>
    (decimal GrossAmount, decimal NetAmount, decimal VatAmount, string InvoiceNumber, DateTime? InvoiceDate)
        ExtractBasicData(string xml);
}