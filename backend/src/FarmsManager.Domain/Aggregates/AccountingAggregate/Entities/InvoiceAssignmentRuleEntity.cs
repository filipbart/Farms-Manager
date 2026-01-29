using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

/// <summary>
/// Reguła automatycznego przypisywania faktur do pracowników.
/// Reguły są sprawdzane w kolejności priorytetu (od najniższego).
/// </summary>
public class InvoiceAssignmentRuleEntity : Entity
{
    protected InvoiceAssignmentRuleEntity()
    {
    }

    /// <summary>
    /// Tworzy nową regułę przypisywania faktur
    /// </summary>
    public static InvoiceAssignmentRuleEntity CreateNew(
        string name,
        int priority,
        Guid assignedUserId,
        string[] includeKeywords,
        string[] excludeKeywords = null,
        Guid? taxBusinessEntityId = null,
        Guid[] farmIds = null,
        string description = null,
        Guid? createdBy = null)
    {
        return new InvoiceAssignmentRuleEntity
        {
            Name = name,
            Priority = priority,
            AssignedUserId = assignedUserId,
            IncludeKeywords = includeKeywords,
            ExcludeKeywords = excludeKeywords ?? [],
            TaxBusinessEntityId = taxBusinessEntityId,
            FarmIds = farmIds ?? [],
            Description = description,
            IsActive = true,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Nazwa reguły (do wyświetlania na liście)
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Opis reguły (opcjonalny)
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Priorytet reguły - niższa wartość = wyższy priorytet.
    /// Reguły są sprawdzane w kolejności od najniższego priorytetu.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Identyfikator pracownika, do którego mają trafiać faktury
    /// </summary>
    public Guid AssignedUserId { get; private set; }

    /// <summary>
    /// Pracownik przypisany do reguły
    /// </summary>
    public virtual UserEntity AssignedUser { get; init; } = null!;

    /// <summary>
    /// Słowa kluczowe, które MUSZĄ się pojawić na fakturze (warunek AND - wszystkie muszą wystąpić)
    /// </summary>
    public string[] IncludeKeywords { get; private set; } = [];

    /// <summary>
    /// Słowa kluczowe wykluczające - jeśli którekolwiek z nich wystąpi, reguła nie zadziała
    /// </summary>
    public string[] ExcludeKeywords { get; private set; } = [];

    /// <summary>
    /// Opcjonalne powiązanie z podmiotem gospodarczym (działalnością)
    /// </summary>
    public Guid? TaxBusinessEntityId { get; private set; }

    /// <summary>
    /// Podmiot gospodarczy powiązany z regułą
    /// </summary>
    public virtual TaxBusinessEntity TaxBusinessEntity { get; init; }

    /// <summary>
    /// Opcjonalne powiązanie z fermami (lokalizacjami)
    /// Pusta tablica = wszystkie fermy
    /// </summary>
    public Guid[] FarmIds { get; private set; } = [];

    /// <summary>
    /// Czy reguła jest aktywna
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Aktualizuje regułę
    /// </summary>
    public void Update(
        string name = null,
        string description = null,
        int? priority = null,
        Guid? assignedUserId = null,
        string[] includeKeywords = null,
        string[] excludeKeywords = null,
        Guid? taxBusinessEntityId = null,
        Guid[] farmIds = null,
        bool? isActive = null)
    {
        if (name != null)
            Name = name;

        if (description != null)
            Description = description;

        if (priority.HasValue)
            Priority = priority.Value;

        if (assignedUserId.HasValue)
            AssignedUserId = assignedUserId.Value;

        if (includeKeywords != null)
            IncludeKeywords = includeKeywords;

        if (excludeKeywords != null)
            ExcludeKeywords = excludeKeywords;

        if (taxBusinessEntityId.HasValue)
            TaxBusinessEntityId = taxBusinessEntityId.Value == Guid.Empty ? null : taxBusinessEntityId.Value;

        if (farmIds != null)
            FarmIds = farmIds;

        if (isActive.HasValue)
            IsActive = isActive.Value;
    }

    /// <summary>
    /// Zmienia priorytet reguły
    /// </summary>
    public void SetPriority(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    /// Aktywuje regułę
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Dezaktywuje regułę
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Sprawdza czy faktura pasuje do tej reguły.
    /// </summary>
    /// <param name="searchableText">Tekst do przeszukania (połączone pola faktury)</param>
    /// <param name="invoiceTaxBusinessEntityId">ID podmiotu gospodarczego faktury</param>
    /// <param name="invoiceFarmId">ID fermy faktury</param>
    /// <returns>True jeśli faktura pasuje do reguły</returns>
    public bool MatchesInvoice(
        string searchableText,
        Guid? invoiceTaxBusinessEntityId = null,
        Guid? invoiceFarmId = null)
    {
        if (!IsActive)
            return false;

        // Sprawdź powiązanie z podmiotem gospodarczym
        if (TaxBusinessEntityId.HasValue && TaxBusinessEntityId != invoiceTaxBusinessEntityId)
            return false;

        // Sprawdź powiązanie z fermami (OR - którakolwiek z wybranych)
        // Jeśli FarmIds jest puste = wszystkie fermy
        // Jeśli FarmIds ma wartości = faktura musi mieć jedną z tych ferm
        if (FarmIds.Length > 0 && invoiceFarmId.HasValue)
        {
            if (!FarmIds.Contains(invoiceFarmId.Value))
                return false;
        }

        var textLower = searchableText.ToLowerInvariant();

        // Sprawdź słowa wykluczające - jeśli którekolwiek występuje, reguła nie pasuje
        foreach (var excludeKeyword in ExcludeKeywords)
        {
            if (!string.IsNullOrWhiteSpace(excludeKeyword) &&
                textLower.Contains(excludeKeyword.ToLowerInvariant()))
            {
                return false;
            }
        }

        // Sprawdź słowa wymagane - wystarczy jedno z podanych
        var hasMatchingIncludeKeyword = false;
        foreach (var includeKeyword in IncludeKeywords)
        {
            if (!string.IsNullOrWhiteSpace(includeKeyword) &&
                textLower.Contains(includeKeyword.ToLowerInvariant()))
            {
                hasMatchingIncludeKeyword = true;
                break;
            }
        }

        // Jeśli są zdefiniowane słowa kluczowe i żadne nie pasuje, reguła nie jest spełniona
        if (IncludeKeywords.Any(k => !string.IsNullOrWhiteSpace(k)) && !hasMatchingIncludeKeyword)
        {
            return false;
        }

        // Jeśli nie ma słów kluczowych wymaganych, reguła nie może działać samodzielnie
        // (musi mieć przynajmniej jedno słowo kluczowe lub powiązanie z podmiotem/fermami)
        if (IncludeKeywords.Length == 0 && !TaxBusinessEntityId.HasValue && FarmIds.Length == 0)
            return false;

        return true;
    }
}
