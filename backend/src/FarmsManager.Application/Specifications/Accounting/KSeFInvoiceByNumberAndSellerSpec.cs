using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceByNumberAndSellerSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceByNumberAndSellerSpec(string invoiceNumber, string normalizedSellerNip, Guid? taxBusinessEntityId, string normalizedBuyerNip = null)
    {
        DisableTracking();
        EnsureExists();

        if (taxBusinessEntityId.HasValue)
        {
            Query.Where(i =>
                i.InvoiceNumber == invoiceNumber &&
                i.Status != KSeFInvoiceStatus.Rejected &&
                (i.SellerNip == normalizedSellerNip || i.TaxBusinessEntityId == taxBusinessEntityId) &&
                (normalizedBuyerNip == null || i.BuyerNip == normalizedBuyerNip));
        }
        else
        {
            Query.Where(i =>
                i.InvoiceNumber == invoiceNumber &&
                i.Status != KSeFInvoiceStatus.Rejected &&
                i.SellerNip == normalizedSellerNip &&
                (normalizedBuyerNip == null || i.BuyerNip == normalizedBuyerNip));
        }
    }
}