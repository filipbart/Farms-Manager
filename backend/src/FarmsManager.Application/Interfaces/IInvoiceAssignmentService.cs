using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Interfaces;

/// <summary>
/// Serwis do automatycznego przypisywania faktur do pracowników i modułów na podstawie reguł
/// </summary>
public interface IInvoiceAssignmentService
{
    /// <summary>
    /// Znajduje odpowiednią regułę dla faktury i zwraca ID użytkownika do przypisania
    /// </summary>
    /// <param name="invoice">Faktura do dopasowania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>ID użytkownika do przypisania lub null jeśli nie znaleziono pasującej reguły</returns>
    Task<Guid?> FindAssignedUserForInvoiceAsync(KSeFInvoiceEntity invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Znajduje odpowiednią regułę dla faktury i zwraca moduł do przypisania
    /// </summary>
    /// <param name="invoice">Faktura do dopasowania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Moduł do przypisania lub null jeśli nie znaleziono pasującej reguły</returns>
    Task<ModuleType?> FindModuleForInvoiceAsync(KSeFInvoiceEntity invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Buduje tekst do przeszukiwania z faktury (łączy wszystkie przeszukiwane pola)
    /// </summary>
    string BuildSearchableText(KSeFInvoiceEntity invoice);
}
