using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;
using KSeF.Client.Core.Models.Invoices.Common;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

public class KSeFInvoiceEntity : Entity
{
    protected KSeFInvoiceEntity()
    {
    }

    /// <summary>
    /// Fabryka tworząca nową encję faktury KSeF zgodnie z konwencją projektu.
    /// </summary>
    /// <param name="kSeFNumber">Numer KSeF</param>
    /// <param name="invoiceNumber">Numer faktury</param>
    /// <param name="invoiceDate">Data wystawienia</param>
    /// <param name="sellerNip">NIP sprzedawcy</param>
    /// <param name="sellerName">Nazwa sprzedawcy</param>
    /// <param name="buyerNip">NIP nabywcy</param>
    /// <param name="buyerName">Nazwa nabywcy</param>
    /// <param name="invoiceType">Typ faktury</param>
    /// <param name="status">Status faktury w KSeF</param>
    /// <param name="paymentStatus">Status płatności</param>
    /// <param name="paymentType">Forma płatności</param>
    /// <param name="vatDeductionType">Typ odliczenia VAT</param>
    /// <param name="moduleType">Przypisany moduł</param>
    /// <param name="invoiceXml">Oryginalny XML faktury</param>
    /// <param name="invoiceDirection">Kierunek faktury (sprzedaż/zakup)</param>
    /// <param name="invoiceSource">Źródło faktury (KSeF/Manual)</param>
    /// <param name="grossAmount">Kwota brutto</param>
    /// <param name="netAmount">Kwota netto</param>
    /// <param name="vatAmount">Kwota VAT</param>
    /// <param name="assignedUserId">Id użytkownika przypisanego do faktury (opcjonalnie)</param>
    /// <param name="relatedInvoiceNumber">Powiązany numer faktury (opcjonalnie)</param>
    /// <param name="relatedInvoiceId">Powiązana faktura w systemie (opcjonalnie)</param>
    /// <param name="comment">Komentarz (opcjonalnie)</param>
    /// <param name="userId">Identyfikator użytkownika tworzącego rekord (audit) (opcjonalnie)</param>
    /// <param name="taxBusinessEntityId">Identyfikator podmiotu gospodarczego (opcjonalnie)</param>
    public static KSeFInvoiceEntity CreateNew(
        string kSeFNumber,
        string invoiceNumber,
        DateOnly invoiceDate,
        string sellerNip,
        string sellerName,
        string buyerNip,
        string buyerName,
        InvoiceType invoiceType,
        KSeFInvoiceStatus status,
        KSeFPaymentStatus paymentStatus,
        KSeFInvoicePaymentType paymentType,
        KSeFVatDeductionType vatDeductionType,
        ModuleType moduleType,
        string invoiceXml,
        KSeFInvoiceDirection invoiceDirection,
        KSeFInvoiceSource invoiceSource,
        decimal grossAmount,
        decimal netAmount,
        decimal vatAmount,
        Guid? assignedUserId = null,
        string relatedInvoiceNumber = null,
        Guid? relatedInvoiceId = null,
        string comment = null,
        Guid? userId = null,
        Guid? taxBusinessEntityId = null)
    {
        return new KSeFInvoiceEntity
        {
            KSeFNumber = kSeFNumber,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            SellerNip = sellerNip,
            SellerName = sellerName,
            BuyerNip = buyerNip,
            BuyerName = buyerName,
            InvoiceType = invoiceType,
            Status = status,
            PaymentStatus = paymentStatus,
            PaymentType = paymentType,
            VatDeductionType = vatDeductionType,
            ModuleType = moduleType,
            InvoiceXml = invoiceXml,
            InvoiceDirection = invoiceDirection,
            InvoiceSource = invoiceSource,
            GrossAmount = grossAmount,
            NetAmount = netAmount,
            VatAmount = vatAmount,
            AssignedUserId = assignedUserId,
            RelatedInvoiceNumber = relatedInvoiceNumber,
            RelatedInvoiceId = relatedInvoiceId,
            Comment = comment,
            CreatedBy = userId,
            TaxBusinessEntityId = taxBusinessEntityId
        };
    }

    /// <summary>
    /// Numer faktury KSEF
    /// </summary>
    public string KSeFNumber { get; init; }

    /// <summary>
    /// Numer faktury
    /// </summary>
    public string InvoiceNumber { get; init; }

    /// <summary>
    /// Data wystawienia faktury
    /// </summary>
    public DateOnly InvoiceDate { get; init; }

    /// <summary>
    /// NIP sprzedawcy
    /// </summary>
    public string SellerNip { get; init; }

    /// <summary>
    /// Nazwa sprzedawcy
    /// </summary>
    public string SellerName { get; init; }

    /// <summary>
    /// NIP nabywcy
    /// </summary>
    public string BuyerNip { get; init; }

    /// <summary>
    /// Nazwa nabywcy
    /// </summary>
    public string BuyerName { get; init; }

    /// <summary>
    /// Typ faktury (np. podstawowa, zaliczkowa, korygująca)
    /// </summary>
    public InvoiceType InvoiceType { get; init; }

    /// <summary>
    /// Status faktury w KSeF
    /// </summary>
    public KSeFInvoiceStatus Status { get; init; }

    /// <summary>
    /// Status płatności faktury
    /// </summary>
    public KSeFPaymentStatus PaymentStatus { get; init; }

    /// <summary>
    /// Forma płatności faktury
    /// </summary>
    public KSeFInvoicePaymentType PaymentType { get; init; }

    /// <summary>
    /// Typ odliczenia VAT
    /// </summary>
    public KSeFVatDeductionType VatDeductionType { get; init; }

    /// <summary>
    /// Przypisany moduł
    /// </summary>
    public ModuleType ModuleType { get; init; }

    /// <summary>
    /// Kwota brutto faktury
    /// </summary>
    public decimal GrossAmount { get; init; }

    /// <summary>
    /// Kwota netto faktury
    /// </summary>
    public decimal NetAmount { get; init; }

    /// <summary>
    /// Kwota VAT faktury
    /// </summary>
    public decimal VatAmount { get; init; }

    /// <summary>
    /// Kierunek faktury (sprzedaż/zakup)
    /// </summary>
    public KSeFInvoiceDirection InvoiceDirection { get; init; }

    /// <summary>
    /// Źródło faktury (KSeF/Manual)
    /// </summary>
    public KSeFInvoiceSource InvoiceSource { get; init; }

    /// <summary>
    /// Numer powiązanej faktury
    /// </summary>
    public string RelatedInvoiceNumber { get; init; }

    /// <summary>
    /// Identyfikator powiązanej faktury w systemie
    /// </summary>
    public Guid? RelatedInvoiceId { get; init; }

    /// <summary>
    /// Dodatkowy komentarz do faktury
    /// </summary>
    public string Comment { get; init; }

    /// <summary>
    /// Zawartość faktury w formacie XML (oryginał z KSeF)
    /// </summary>
    public string InvoiceXml { get; init; }

    /// <summary>
    /// Identyfikator pracownika przypisanego do faktury 
    /// </summary>
    public Guid? AssignedUserId { get; init; }

    /// <summary>
    /// Identyfikator cyklu przypisanego do faktury
    /// </summary>
    public Guid? AssignedCycleId { get; init; }

    /// <summary>
    /// Identyfikator encji faktury
    /// </summary>
    public Guid? AssignedEntityInvoiceId { get; init; }

    /// <summary>
    /// Pracownik przypisany do faktury
    /// </summary>
    public virtual UserEntity AssignedUser { get; init; }

    /// <summary>
    /// Cykl przypisany do faktury
    /// </summary>
    public virtual CycleEntity AssignedCycle { get; init; }

    /// <summary>
    /// Identyfikator podmiotu gospodarczego przypisanego do faktury
    /// </summary>
    public Guid? TaxBusinessEntityId { get; init; }

    /// <summary>
    /// Podmiot gospodarczy przypisany do faktury
    /// </summary>
    public virtual TaxBusinessEntity TaxBusinessEntity { get; init; }
}