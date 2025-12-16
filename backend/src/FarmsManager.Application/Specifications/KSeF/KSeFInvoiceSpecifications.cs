using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.KSeF;

/// <summary>
/// Specyfikacja do pobierania faktur KSeF po numerach KSeF
/// </summary>
public sealed class GetKSeFInvoicesByNumbersSpec : BaseSpecification<KSeFInvoiceEntity, string>
{
    public GetKSeFInvoicesByNumbersSpec(IEnumerable<string> ksefNumbers)
    {
        EnsureExists();
        Query
            .Where(i => ksefNumbers.Contains(i.KSeFNumber))
            .Select(i => i.KSeFNumber);
    }
}

/// <summary>
/// Specyfikacja do sprawdzenia czy faktura KSeF o danym numerze już istnieje
/// </summary>
public sealed class KSeFInvoiceExistsByNumberSpec : BaseSpecification<KSeFInvoiceEntity>,
    ISingleResultSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceExistsByNumberSpec(string ksefNumber)
    {
        EnsureExists();
        Query.Where(i => i.KSeFNumber == ksefNumber);
    }
}
