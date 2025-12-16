using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

/// <summary>
/// Podmiot gospodarczy - reprezentuje działalność pod danym NIP-em
/// Jeden NIP może mieć wiele działalności (np. Ferma Drobiu, Gospodarstwo Rolne, Najem)
/// </summary>
public class TaxBusinessEntity : Entity
{
    protected TaxBusinessEntity()
    {
    }

    /// <summary>
    /// NIP podmiotu
    /// </summary>
    public string Nip { get; protected internal set; }

    /// <summary>
    /// Nazwa działalności/firmy
    /// </summary>
    public string Name { get; protected internal set; }

    /// <summary>
    /// Typ działalności (np. "Ferma Drobiu", "Gospodarstwo Rolne", "Najem mieszkań")
    /// </summary>
    public string BusinessType { get; protected internal set; }

    /// <summary>
    /// Opis dodatkowy (opcjonalny)
    /// </summary>
    public string Description { get; protected internal set; }

    /// <summary>
    /// Faktury KSeF przypisane do tego podmiotu
    /// </summary>
    private readonly List<KSeFInvoiceEntity> _invoices = [];
    public virtual IReadOnlyCollection<KSeFInvoiceEntity> Invoices => _invoices.AsReadOnly();

    /// <summary>
    /// Fermy przypisane do tego podmiotu
    /// </summary>
    private readonly List<FarmEntity> _farms = [];
    public virtual IReadOnlyCollection<FarmEntity> Farms => _farms.AsReadOnly();

    /// <summary>
    /// Tworzy nowy podmiot gospodarczy
    /// </summary>
    public static TaxBusinessEntity CreateNew(
        string nip,
        string name,
        string businessType,
        string description = null,
        Guid? createdBy = null)
    {
        return new TaxBusinessEntity
        {
            Nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            Name = name,
            BusinessType = businessType,
            Description = description,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Aktualizuje dane podmiotu
    /// </summary>
    public void Update(string name, string businessType, string description = null)
    {
        Name = name;
        BusinessType = businessType;
        Description = description;
    }
}
