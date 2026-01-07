using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

/// <summary>
/// Reguła automatycznego przypisywania faktur do modułów.
/// Reguły są sprawdzane w kolejności priorytetu (od najniższego).
/// </summary>
public class InvoiceModuleAssignmentRuleEntity : Entity
{
    protected InvoiceModuleAssignmentRuleEntity()
    {
    }

    /// <summary>
    /// Tworzy nową regułę przypisywania faktur do modułów
    /// </summary>
    public static InvoiceModuleAssignmentRuleEntity CreateNew(
        string name,
        int priority,
        ModuleType targetModule,
        string[] includeKeywords,
        string[]? excludeKeywords = null,
        Guid? taxBusinessEntityId = null,
        Guid? farmId = null,
        KSeFInvoiceDirection? invoiceDirection = null,
        string? description = null,
        Guid? createdBy = null)
    {
        return new InvoiceModuleAssignmentRuleEntity
        {
            Name = name,
            Priority = priority,
            TargetModule = targetModule,
            IncludeKeywords = includeKeywords,
            ExcludeKeywords = excludeKeywords ?? Array.Empty<string>(),
            TaxBusinessEntityId = taxBusinessEntityId,
            FarmId = farmId,
            InvoiceDirection = invoiceDirection,
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
    public string? Description { get; private set; }

    /// <summary>
    /// Priorytet reguły - niższa wartość = wyższy priorytet.
    /// Reguły są sprawdzane w kolejności od najniższego priorytetu.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Moduł docelowy, do którego mają trafiać faktury
    /// </summary>
    public ModuleType TargetModule { get; private set; }

    /// <summary>
    /// Słowa kluczowe, które MUSZĄ się pojawić na fakturze (warunek OR - przynajmniej jedno musi wystąpić)
    /// </summary>
    public string[] IncludeKeywords { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Słowa kluczowe wykluczające - jeśli którekolwiek z nich wystąpi, reguła nie zadziała
    /// </summary>
    public string[] ExcludeKeywords { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Opcjonalne powiązanie z podmiotem gospodarczym (działalnością)
    /// </summary>
    public Guid? TaxBusinessEntityId { get; private set; }

    /// <summary>
    /// Podmiot gospodarczy powiązany z regułą
    /// </summary>
    public virtual TaxBusinessEntity? TaxBusinessEntity { get; init; }

    /// <summary>
    /// Opcjonalne powiązanie z fermą (lokalizacją)
    /// </summary>
    public Guid? FarmId { get; private set; }

    /// <summary>
    /// Ferma powiązana z regułą
    /// </summary>
    public virtual FarmEntity? Farm { get; init; }

    /// <summary>
    /// Opcjonalny kierunek faktury (zakup/sprzedaż) - jeśli null, pasuje do obu
    /// </summary>
    public KSeFInvoiceDirection? InvoiceDirection { get; private set; }

    /// <summary>
    /// Czy reguła jest aktywna
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Aktualizuje regułę
    /// </summary>
    public void Update(
        string? name = null,
        string? description = null,
        int? priority = null,
        ModuleType? targetModule = null,
        string[]? includeKeywords = null,
        string[]? excludeKeywords = null,
        Guid? taxBusinessEntityId = null,
        Guid? farmId = null,
        KSeFInvoiceDirection? invoiceDirection = null,
        bool? isActive = null,
        bool clearTaxBusinessEntity = false,
        bool clearFarm = false,
        bool clearInvoiceDirection = false)
    {
        if (name != null)
            Name = name;

        if (description != null)
            Description = description;

        if (priority.HasValue)
            Priority = priority.Value;

        if (targetModule.HasValue)
            TargetModule = targetModule.Value;

        if (includeKeywords != null)
            IncludeKeywords = includeKeywords;

        if (excludeKeywords != null)
            ExcludeKeywords = excludeKeywords;

        if (clearTaxBusinessEntity)
            TaxBusinessEntityId = null;
        else if (taxBusinessEntityId.HasValue)
            TaxBusinessEntityId = taxBusinessEntityId.Value == Guid.Empty ? null : taxBusinessEntityId.Value;

        if (clearFarm)
            FarmId = null;
        else if (farmId.HasValue)
            FarmId = farmId.Value == Guid.Empty ? null : farmId.Value;

        if (clearInvoiceDirection)
            InvoiceDirection = null;
        else if (invoiceDirection.HasValue)
            InvoiceDirection = invoiceDirection.Value;

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
    /// <param name="invoiceDirection">Kierunek faktury (zakup/sprzedaż)</param>
    /// <returns>True jeśli faktura pasuje do reguły</returns>
    public bool MatchesInvoice(
        string searchableText,
        Guid? invoiceTaxBusinessEntityId = null,
        Guid? invoiceFarmId = null,
        KSeFInvoiceDirection? invoiceDirection = null)
    {
        if (!IsActive)
            return false;

        // Sprawdź powiązanie z podmiotem gospodarczym
        if (TaxBusinessEntityId.HasValue && TaxBusinessEntityId != invoiceTaxBusinessEntityId)
            return false;

        // Sprawdź powiązanie z fermą
        if (FarmId.HasValue && FarmId != invoiceFarmId)
            return false;

        // Sprawdź kierunek faktury
        if (InvoiceDirection.HasValue && invoiceDirection.HasValue && InvoiceDirection != invoiceDirection)
            return false;

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

        // Sprawdź słowa wymagane - przynajmniej jedno musi występować (OR)
        if (IncludeKeywords.Length > 0)
        {
            var hasMatch = false;
            foreach (var includeKeyword in IncludeKeywords)
            {
                if (!string.IsNullOrWhiteSpace(includeKeyword) &&
                    textLower.Contains(includeKeyword.ToLowerInvariant()))
                {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch)
                return false;
        }

        // Jeśli nie ma słów kluczowych wymaganych, reguła nie może działać samodzielnie
        // (musi mieć przynajmniej jedno słowo kluczowe lub powiązanie z podmiotem/fermą/kierunkiem)
        if (IncludeKeywords.Length == 0 && !TaxBusinessEntityId.HasValue && !FarmId.HasValue && !InvoiceDirection.HasValue)
            return false;

        return true;
    }
}
