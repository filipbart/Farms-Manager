using AutoMapper;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.XmlModels;

namespace FarmsManager.Application.Mappings;

public class KSeFInvoiceProfile : Profile
{
    public KSeFInvoiceProfile()
    {
        // Entity -> DTO dla listy faktur
        CreateMap<KSeFInvoiceEntity, KSeFInvoiceFromDbDto>()
            .ForMember(d => d.Nip, opt => opt.MapFrom(s => 
                s.InvoiceDirection == KSeFInvoiceDirection.Sales ? s.BuyerNip : s.SellerNip))
            .ForMember(d => d.InvoiceType, opt => opt.MapFrom(s => s.InvoiceDirection.ToString()))
            .ForMember(d => d.CycleIdentifier, opt => opt.MapFrom(s => s.AssignedCycle != null ? s.AssignedCycle.Identifier : (int?)null))
            .ForMember(d => d.CycleYear, opt => opt.MapFrom(s => s.AssignedCycle != null ? s.AssignedCycle.Year : (int?)null))
            .ForMember(d => d.Source, opt => opt.MapFrom(s => s.InvoiceSource.ToString()))
            .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Farm != null ? s.Farm.Name : null))
            .ForMember(d => d.ModuleType, opt => opt.MapFrom(s => s.ModuleType.ToString()))
            .ForMember(d => d.VatDeductionType, opt => opt.MapFrom(s => s.VatDeductionType.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.PaymentStatus.ToString()))
            .ForMember(d => d.PaymentType, opt => opt.MapFrom(s => s.PaymentType.ToString()))
            .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity))
            .ForMember(d => d.HasXml, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.InvoiceXml)))
            .ForMember(d => d.HasPdf, opt => opt.MapFrom(s => false))
            .ForMember(d => d.AssignedUserId, opt => opt.MapFrom(s => s.AssignedUserId))
            .ForMember(d => d.AssignedUserName, opt => opt.MapFrom(s => s.AssignedUser != null ? s.AssignedUser.Name : null))
            .ForMember(d => d.PaymentDate, opt => opt.MapFrom(s => s.PaymentDate))
            .ForMember(d => d.DateCreatedUtc, opt => opt.MapFrom(s => s.DateCreatedUtc))
            .ForMember(d => d.DaysUntilDue, opt => opt.MapFrom(s => 
                s.PaymentDueDate.HasValue 
                    ? (int?)(s.PaymentDueDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).TotalDays 
                    : null));

        // Entity -> DTO dla szczegółów faktury
        CreateMap<KSeFInvoiceEntity, KSeFInvoiceDetailsDto>()
            .ForMember(d => d.Nip, opt => opt.MapFrom(s => 
                s.InvoiceDirection == KSeFInvoiceDirection.Sales ? s.BuyerNip : s.SellerNip))
            .ForMember(d => d.InvoiceType, opt => opt.MapFrom(s => s.InvoiceDirection.ToString()))
            .ForMember(d => d.CycleIdentifier, opt => opt.MapFrom(s => s.AssignedCycle != null ? s.AssignedCycle.Identifier : (int?)null))
            .ForMember(d => d.CycleYear, opt => opt.MapFrom(s => s.AssignedCycle != null ? s.AssignedCycle.Year : (int?)null))
            .ForMember(d => d.CycleId, opt => opt.MapFrom(s => s.AssignedCycleId))
            .ForMember(d => d.Source, opt => opt.MapFrom(s => s.InvoiceSource.ToString()))
            .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Farm != null ? s.Farm.Name : null))
            .ForMember(d => d.FarmId, opt => opt.MapFrom(s => s.FarmId))
            .ForMember(d => d.ModuleType, opt => opt.MapFrom(s => s.ModuleType.ToString()))
            .ForMember(d => d.VatDeductionType, opt => opt.MapFrom(s => s.VatDeductionType.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.PaymentStatus.ToString()))
            .ForMember(d => d.PaymentType, opt => opt.MapFrom(s => s.PaymentType.ToString()))
            .ForMember(d => d.HasXml, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.InvoiceXml)))
            .ForMember(d => d.HasPdf, opt => opt.MapFrom(s => false))
            .ForMember(d => d.AssignedUserId, opt => opt.MapFrom(s => s.AssignedUserId))
            .ForMember(d => d.AssignedUserName, opt => opt.MapFrom(s => s.AssignedUser != null ? s.AssignedUser.Name : null))
            .ForMember(d => d.AssignedEntityInvoiceId, opt => opt.MapFrom(s => s.AssignedEntityInvoiceId))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.DateCreatedUtc))
            .ForMember(d => d.CreatedBy, opt => opt.MapFrom(s => s.Creator != null ? s.Creator.Name : null))
            .ForMember(d => d.PaymentDate, opt => opt.MapFrom(s => s.PaymentDate))
            .ForMember(d => d.AttachmentsCount, opt => opt.MapFrom(s => s.Attachments != null ? s.Attachments.Count(a => a.DateDeletedUtc == null) : 0))
            .ForMember(d => d.AuditLogsCount, opt => opt.MapFrom(s => s.AuditLogs != null ? s.AuditLogs.Count(a => a.DateDeletedUtc == null) : 0));

        // XML Model -> KSeFInvoiceDetails (dla parsowania XML z KSeF API)
        CreateMap<KSeFInvoiceXml, KSeFInvoiceDetails>()
            .ForMember(d => d.ReferenceNumber, opt => opt.Ignore()) // Ustawiane ręcznie
            .ForMember(d => d.InvoiceNumber, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.P_2 : null))
            .ForMember(d => d.InvoiceDate, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.P_1 : DateTime.MinValue))
            .ForMember(d => d.SaleDate, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.P_6 : null))
            .ForMember(d => d.DueDate, opt => opt.MapFrom(s => 
                s.Fa != null && s.Fa.Platnosc != null && s.Fa.Platnosc.TerminyPlatnosci != null && s.Fa.Platnosc.TerminyPlatnosci.Count > 0
                    ? s.Fa.Platnosc.TerminyPlatnosci[0].Termin : null))
            .ForMember(d => d.GrossAmount, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.P_15 : 0))
            .ForMember(d => d.NetAmount, opt => opt.MapFrom<NetAmountResolver>())
            .ForMember(d => d.VatAmount, opt => opt.MapFrom<VatAmountResolver>())
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.KodWaluty ?? "PLN" : "PLN"))
            .ForMember(d => d.SellerNip, opt => opt.MapFrom(s => 
                s.Podmiot1 != null && s.Podmiot1.DaneIdentyfikacyjne != null ? s.Podmiot1.DaneIdentyfikacyjne.NIP : null))
            .ForMember(d => d.SellerName, opt => opt.MapFrom(s => 
                s.Podmiot1 != null && s.Podmiot1.DaneIdentyfikacyjne != null ? s.Podmiot1.DaneIdentyfikacyjne.Nazwa : null))
            .ForMember(d => d.SellerAddress, opt => opt.MapFrom<SellerAddressResolver>())
            .ForMember(d => d.BuyerNip, opt => opt.MapFrom(s => 
                s.Podmiot2 != null && s.Podmiot2.DaneIdentyfikacyjne != null ? s.Podmiot2.DaneIdentyfikacyjne.NIP : null))
            .ForMember(d => d.BuyerName, opt => opt.MapFrom(s => 
                s.Podmiot2 != null && s.Podmiot2.DaneIdentyfikacyjne != null ? s.Podmiot2.DaneIdentyfikacyjne.Nazwa : null))
            .ForMember(d => d.BuyerAddress, opt => opt.MapFrom<BuyerAddressResolver>())
            .ForMember(d => d.ReceivedDate, opt => opt.MapFrom(s => 
                s.Naglowek != null ? s.Naglowek.DataWytworzeniaFa : DateTime.MinValue))
            .ForMember(d => d.LineItems, opt => opt.MapFrom(s => s.Fa != null ? s.Fa.FaWiersze : null));

        // FaWiersz -> KSeFInvoiceLineItem
        CreateMap<FaWiersz, KSeFInvoiceLineItem>()
            .ForMember(d => d.LineNumber, opt => opt.MapFrom(s => s.NrWierszaFa))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.P_7))
            .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.P_8B ?? 0))
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.P_8A))
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(s => s.P_9A ?? 0))
            .ForMember(d => d.NetAmount, opt => opt.MapFrom(s => s.P_11 ?? 0))
            .ForMember(d => d.VatRate, opt => opt.MapFrom(s => ParseVatRate(s.P_12)))
            .ForMember(d => d.VatAmount, opt => opt.MapFrom(s =>
                s.P_11Vat ?? (s.P_11.HasValue ? (s.P_11.Value * ParseVatRate(s.P_12)) / 100 : 0)))
            .ForMember(d => d.GrossAmount, opt => opt.MapFrom(s =>
                s.P_11A ?? (s.P_11 ?? 0) + (s.P_11Vat ?? (s.P_11.HasValue ? (s.P_11.Value * ParseVatRate(s.P_12)) / 100 : 0))));
    }

    private static decimal ParseVatRate(string vatRate)
    {
        if (string.IsNullOrWhiteSpace(vatRate))
        {
            return 0;
        }

        // "zw" oznacza zwolniony z VAT (stawka 0%)
        if (vatRate.Equals("zw", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }
        
        return decimal.TryParse(vatRate, out var value) ? value : 0;
    }
}

// Resolver dla kwoty netto (suma P_13_x)
public class NetAmountResolver : IValueResolver<KSeFInvoiceXml, KSeFInvoiceDetails, decimal>
{
    public decimal Resolve(KSeFInvoiceXml source, KSeFInvoiceDetails destination, decimal destMember, ResolutionContext context)
    {
        if (source.Fa == null) return 0;
        var fa = source.Fa;
        return (fa.P_13_1 ?? 0) + (fa.P_13_2 ?? 0) + (fa.P_13_3 ?? 0) + 
               (fa.P_13_4 ?? 0) + (fa.P_13_5 ?? 0) + (fa.P_13_6_1 ?? 0);
    }
}

// Resolver dla kwoty VAT (suma P_14_x)
public class VatAmountResolver : IValueResolver<KSeFInvoiceXml, KSeFInvoiceDetails, decimal>
{
    public decimal Resolve(KSeFInvoiceXml source, KSeFInvoiceDetails destination, decimal destMember, ResolutionContext context)
    {
        if (source.Fa == null) return 0;
        var fa = source.Fa;
        return (fa.P_14_1 ?? 0) + (fa.P_14_2 ?? 0) + (fa.P_14_3 ?? 0);
    }
}

// Resolver dla adresu sprzedawcy
public class SellerAddressResolver : IValueResolver<KSeFInvoiceXml, KSeFInvoiceDetails, string>
{
    public string Resolve(KSeFInvoiceXml source, KSeFInvoiceDetails destination, string destMember, ResolutionContext context)
    {
        var adres = source.Podmiot1?.Adres;
        if (adres == null) return null;
        var parts = new[] { adres.AdresL1, adres.AdresL2, adres.KodKraju }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }
}

// Resolver dla adresu nabywcy
public class BuyerAddressResolver : IValueResolver<KSeFInvoiceXml, KSeFInvoiceDetails, string>
{
    public string Resolve(KSeFInvoiceXml source, KSeFInvoiceDetails destination, string destMember, ResolutionContext context)
    {
        var adres = source.Podmiot2?.Adres;
        if (adres == null) return null;
        var parts = new[] { adres.AdresL1, adres.AdresL2, adres.KodKraju }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }
}
