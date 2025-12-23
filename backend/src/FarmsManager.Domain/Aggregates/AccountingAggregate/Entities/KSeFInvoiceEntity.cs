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
    /// Termin płatności faktury
    /// </summary>
    public DateOnly? PaymentDueDate { get; init; }

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
    public KSeFInvoiceStatus Status { get; private set; }

    /// <summary>
    /// Status płatności faktury
    /// </summary>
    public KSeFPaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Forma płatności faktury
    /// </summary>
    public KSeFInvoicePaymentType PaymentType { get; init; }

    /// <summary>
    /// Typ odliczenia VAT
    /// </summary>
    public KSeFVatDeductionType VatDeductionType { get; private set; }

    /// <summary>
    /// Przypisany moduł
    /// </summary>
    public ModuleType ModuleType { get; private set; }

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
    public string RelatedInvoiceNumber { get; private set; }

    /// <summary>
    /// Identyfikator powiązanej faktury w systemie
    /// </summary>
    public Guid? RelatedInvoiceId { get; init; }

    /// <summary>
    /// Dodatkowy komentarz do faktury
    /// </summary>
    public string Comment { get; private set; }

    /// <summary>
    /// Zawartość faktury w formacie XML (oryginał z KSeF)
    /// </summary>
    public string InvoiceXml { get; init; }

    /// <summary>
    /// Identyfikator pracownika przypisanego do faktury 
    /// </summary>
    public Guid? AssignedUserId { get; private set; }

    /// <summary>
    /// Identyfikator cyklu przypisanego do faktury
    /// </summary>
    public Guid? AssignedCycleId { get; private set; }

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
    /// Identyfikator fermy (lokalizacji) przypisanej do faktury
    /// </summary>
    public Guid? FarmId { get; private set; }

    /// <summary>
    /// Ferma (lokalizacja) przypisana do faktury
    /// </summary>
    public virtual FarmEntity Farm { get; init; }

    /// <summary>
    /// Identyfikator podmiotu gospodarczego przypisanego do faktury
    /// </summary>
    public Guid? TaxBusinessEntityId { get; init; }

    /// <summary>
    /// Podmiot gospodarczy przypisany do faktury
    /// </summary>
    public virtual TaxBusinessEntity TaxBusinessEntity { get; init; }

    /// <summary>
    /// Czy faktura wymaga powiązania z inną fakturą
    /// </summary>
    public bool RequiresLinking { get; private set; }

    /// <summary>
    /// Czy użytkownik zaakceptował brak powiązania
    /// </summary>
    public bool LinkingAccepted { get; private set; }

    /// <summary>
    /// Data następnego przypomnienia o powiązaniu
    /// </summary>
    public DateTime? LinkingReminderDate { get; private set; }

    /// <summary>
    /// Liczba wysłanych przypomnień
    /// </summary>
    public int LinkingReminderCount { get; private set; }

    /// <summary>
    /// Powiązania gdzie ta faktura jest źródłem (np. korekta, zaliczka)
    /// </summary>
    public virtual ICollection<KSeFInvoiceRelationEntity> SourceRelations { get; init; } = new List<KSeFInvoiceRelationEntity>();

    /// <summary>
    /// Powiązania gdzie ta faktura jest celem (np. faktura pierwotna, końcowa)
    /// </summary>
    public virtual ICollection<KSeFInvoiceRelationEntity> TargetRelations { get; init; } = new List<KSeFInvoiceRelationEntity>();

    /// <summary>
    /// Aktualizuje edytowalne pola faktury
    /// </summary>
    public void Update(
        KSeFInvoiceStatus? status = null,
        KSeFPaymentStatus? paymentStatus = null,
        ModuleType? moduleType = null,
        KSeFVatDeductionType? vatDeductionType = null,
        string comment = null,
        Guid? farmId = null,
        Guid? cycleId = null,
        Guid? assignedUserId = null,
        string relatedInvoiceNumber = null)
    {
        if (status.HasValue)
            Status = status.Value;

        if (paymentStatus.HasValue)
            PaymentStatus = paymentStatus.Value;

        if (moduleType.HasValue)
            ModuleType = moduleType.Value;

        if (vatDeductionType.HasValue)
            VatDeductionType = vatDeductionType.Value;

        if (comment != null)
            Comment = comment;

        if (farmId.HasValue)
            FarmId = farmId.Value == Guid.Empty ? null : farmId.Value;

        if (cycleId.HasValue)
            AssignedCycleId = cycleId.Value == Guid.Empty ? null : cycleId.Value;

        if (assignedUserId.HasValue)
            AssignedUserId = assignedUserId.Value == Guid.Empty ? null : assignedUserId.Value;

        if (relatedInvoiceNumber != null)
            RelatedInvoiceNumber = relatedInvoiceNumber;
    }

    /// <summary>
    /// Oznacza fakturę jako wymagającą powiązania
    /// </summary>
    public void MarkAsRequiresLinking()
    {
        RequiresLinking = true;
        Status = KSeFInvoiceStatus.RequiresLinking;
        LinkingReminderDate = DateTime.UtcNow.AddDays(3);
    }

    /// <summary>
    /// Oznacza fakturę jako powiązaną (usuwa wymóg powiązania)
    /// </summary>
    public void MarkAsLinked()
    {
        RequiresLinking = false;
        LinkingAccepted = false;
        LinkingReminderDate = null;
        if (Status == KSeFInvoiceStatus.RequiresLinking)
        {
            Status = KSeFInvoiceStatus.New;
        }
    }

    /// <summary>
    /// Akceptuje brak powiązania (użytkownik świadomie rezygnuje z powiązania)
    /// </summary>
    public void AcceptNoLinking()
    {
        LinkingAccepted = true;
        LinkingReminderDate = null;
        if (Status == KSeFInvoiceStatus.RequiresLinking)
        {
            Status = KSeFInvoiceStatus.New;
        }
    }

    /// <summary>
    /// Odkłada przypomnienie o powiązaniu
    /// </summary>
    public void PostponeLinkingReminder(int days = 3)
    {
        LinkingReminderDate = DateTime.UtcNow.AddDays(days);
        LinkingReminderCount++;
    }
}