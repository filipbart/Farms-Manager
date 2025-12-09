using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;

public sealed class GetKSeFInvoicesFromDbSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public GetKSeFInvoicesFromDbSpec(GetKSeFInvoicesFromDbQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(x => x.AssignedCycle);

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetKSeFInvoicesFromDbQueryFilters filters)
    {
        // Filtrowanie po kierunku faktury
        if (filters.InvoiceType.HasValue && filters.InvoiceType != InvoiceDirectionFilter.All)
        {
            var direction = filters.InvoiceType == InvoiceDirectionFilter.Sales 
                ? KSeFInvoiceDirection.Sales 
                : KSeFInvoiceDirection.Purchase;
            Query.Where(x => x.InvoiceDirection == direction);
        }
        
        // Filtrowanie po źródle
        if (filters.Source.HasValue && filters.Source != InvoiceSourceFilter.All)
        {
            var source = filters.Source == InvoiceSourceFilter.KSeF 
                ? KSeFInvoiceSource.KSeF 
                : KSeFInvoiceSource.Manual;
            Query.Where(x => x.InvoiceSource == source);
        }

        // Filtrowanie po dacie
        if (filters.DateFrom.HasValue)
        {
            Query.Where(x => x.InvoiceDate >= filters.DateFrom.Value);
        }
        
        if (filters.DateTo.HasValue)
        {
            Query.Where(x => x.InvoiceDate <= filters.DateTo.Value);
        }

        // Filtrowanie po statusie
        if (filters.Status.HasValue)
        {
            Query.Where(x => x.Status == filters.Status.Value);
        }
        
        if (filters.PaymentStatus.HasValue)
        {
            Query.Where(x => x.PaymentStatus == filters.PaymentStatus.Value);
        }
        
        if (filters.ModuleType.HasValue)
        {
            Query.Where(x => x.ModuleType == filters.ModuleType.Value);
        }

        // Wyszukiwanie tekstowe
        if (filters.SearchQuery.IsNotEmpty())
        {
            var phrase = $"%{filters.SearchQuery}%";
            Query.Where(x =>
                EF.Functions.ILike(x.InvoiceNumber, phrase) ||
                EF.Functions.ILike(x.KSeFNumber, phrase) ||
                EF.Functions.ILike(x.SellerNip, phrase) ||
                EF.Functions.ILike(x.BuyerNip, phrase) ||
                EF.Functions.ILike(x.SellerName, phrase) ||
                EF.Functions.ILike(x.BuyerName, phrase));
        }
    }

    private void ApplyOrdering(GetKSeFInvoicesFromDbQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case KSeFInvoicesFromDbOrderBy.InvoiceNumber:
                if (isDescending) Query.OrderByDescending(x => x.InvoiceNumber);
                else Query.OrderBy(x => x.InvoiceNumber);
                break;
                
            case KSeFInvoicesFromDbOrderBy.GrossAmount:
                if (isDescending) Query.OrderByDescending(x => x.GrossAmount);
                else Query.OrderBy(x => x.GrossAmount);
                break;
                
            case KSeFInvoicesFromDbOrderBy.SellerName:
                if (isDescending) Query.OrderByDescending(x => x.SellerName);
                else Query.OrderBy(x => x.SellerName);
                break;
                
            case KSeFInvoicesFromDbOrderBy.BuyerName:
                if (isDescending) Query.OrderByDescending(x => x.BuyerName);
                else Query.OrderBy(x => x.BuyerName);
                break;
                
            case KSeFInvoicesFromDbOrderBy.KSeFNumber:
                if (isDescending) Query.OrderByDescending(x => x.KSeFNumber);
                else Query.OrderBy(x => x.KSeFNumber);
                break;
                
            case KSeFInvoicesFromDbOrderBy.Status:
                if (isDescending) Query.OrderByDescending(x => x.Status);
                else Query.OrderBy(x => x.Status);
                break;
                
            case KSeFInvoicesFromDbOrderBy.PaymentStatus:
                if (isDescending) Query.OrderByDescending(x => x.PaymentStatus);
                else Query.OrderBy(x => x.PaymentStatus);
                break;
                
            case KSeFInvoicesFromDbOrderBy.InvoiceDate:
            default:
                if (isDescending) Query.OrderByDescending(x => x.InvoiceDate);
                else Query.OrderBy(x => x.InvoiceDate);
                break;
        }
    }
}
