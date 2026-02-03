using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Specifications.Accounting;

/// <summary>
/// Specyfikacja do wyszukiwania podobnych faktur KSeF
/// Używana do wykrywania potencjalnych duplikatów na podstawie:
/// - tego samego NIP sprzedawcy/nabywcy
/// - podobnej kwoty brutto (w tolerancji procentowej)
/// - daty faktury w określonym zakresie
/// </summary>
public sealed class SimilarKSeFInvoicesSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public SimilarKSeFInvoicesSpec(
        string normalizedSellerNip,
        string normalizedBuyerNip,
        decimal grossAmount,
        DateOnly invoiceDate,
        decimal amountTolerancePercentage = 5.0m,
        int dateRangeDays = 30)
    {
        DisableTracking();
        EnsureExists();

        // Oblicz tolerancję kwoty
        var minAmount = grossAmount * (1 - amountTolerancePercentage / 100);
        var maxAmount = grossAmount * (1 + amountTolerancePercentage / 100);
        
        // Oblicz zakres dat
        var minDate = invoiceDate.AddDays(-dateRangeDays);
        var maxDate = invoiceDate.AddDays(dateRangeDays);

        Query.Where(i =>
            i.Status != KSeFInvoiceStatus.Rejected &&
            (
                // Dopasuj po NIP sprzedawcy lub nabywcy
                (!string.IsNullOrWhiteSpace(normalizedSellerNip) && i.SellerNip == normalizedSellerNip) ||
                (!string.IsNullOrWhiteSpace(normalizedBuyerNip) && i.BuyerNip == normalizedBuyerNip)
            ) &&
            // Podobna kwota brutto
            i.GrossAmount >= minAmount &&
            i.GrossAmount <= maxAmount &&
            // Data w określonym zakresie
            i.InvoiceDate >= minDate &&
            i.InvoiceDate <= maxDate);
    }
}
