using FarmsManager.Application.Common;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

namespace FarmsManager.Application.Services;

/// <summary>
/// Serwis do automatycznego przypisywania faktur do pracowników i modułów na podstawie reguł
/// </summary>
public class InvoiceAssignmentService : IInvoiceAssignmentService, IService
{
    private readonly IInvoiceAssignmentRuleRepository _ruleRepository;
    private readonly IInvoiceModuleAssignmentRuleRepository _moduleRuleRepository;
    private readonly IKSeFInvoiceXmlParser _xmlParser;

    public InvoiceAssignmentService(
        IInvoiceAssignmentRuleRepository ruleRepository,
        IInvoiceModuleAssignmentRuleRepository moduleRuleRepository,
        IKSeFInvoiceXmlParser xmlParser)
    {
        _ruleRepository = ruleRepository;
        _moduleRuleRepository = moduleRuleRepository;
        _xmlParser = xmlParser;
    }

    /// <inheritdoc />
    public async Task<Guid?> FindAssignedUserForInvoiceAsync(
        KSeFInvoiceEntity invoice, 
        CancellationToken cancellationToken = default)
    {
        var rules = await _ruleRepository.GetAllActiveOrderedByPriorityAsync(cancellationToken);
        
        if (rules.Count == 0)
            return null;

        var searchableText = BuildSearchableText(invoice);

        foreach (var rule in rules)
        {
            if (rule.MatchesInvoice(searchableText, invoice.TaxBusinessEntityId, invoice.FarmId))
            {
                return rule.AssignedUserId;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<ModuleType?> FindModuleForInvoiceAsync(
        KSeFInvoiceEntity invoice,
        CancellationToken cancellationToken = default)
    {
        var rules = await _moduleRuleRepository.GetAllActiveOrderedByPriorityAsync(cancellationToken);

        if (rules.Count == 0)
            return null;

        var searchableText = BuildSearchableText(invoice);

        foreach (var rule in rules)
        {
            if (rule.MatchesInvoice(searchableText, invoice.TaxBusinessEntityId, invoice.FarmId, invoice.InvoiceDirection))
            {
                return rule.TargetModule;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public string BuildSearchableText(KSeFInvoiceEntity invoice)
    {
        var parts = new List<string>();

        // Dane podstawowe faktury
        if (!string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            parts.Add(invoice.InvoiceNumber);

        if (!string.IsNullOrWhiteSpace(invoice.SellerName))
            parts.Add(invoice.SellerName);

        if (!string.IsNullOrWhiteSpace(invoice.SellerNip))
            parts.Add(invoice.SellerNip);

        if (!string.IsNullOrWhiteSpace(invoice.BuyerName))
            parts.Add(invoice.BuyerName);

        if (!string.IsNullOrWhiteSpace(invoice.BuyerNip))
            parts.Add(invoice.BuyerNip);

        // Parsowanie XML dla dodatkowych danych
        if (!string.IsNullOrWhiteSpace(invoice.InvoiceXml))
        {
            var parsedXml = _xmlParser.ParseInvoiceXml(invoice.InvoiceXml);
            if (parsedXml != null)
            {
                // Dane sprzedawcy z XML
                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot1?.DaneIdentyfikacyjne?.Nazwa))
                    parts.Add(parsedXml.Podmiot1.DaneIdentyfikacyjne.Nazwa);

                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot1?.Adres?.AdresL1))
                    parts.Add(parsedXml.Podmiot1.Adres.AdresL1);

                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot1?.Adres?.AdresL2))
                    parts.Add(parsedXml.Podmiot1.Adres.AdresL2);

                // Dane nabywcy z XML
                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot2?.DaneIdentyfikacyjne?.Nazwa))
                    parts.Add(parsedXml.Podmiot2.DaneIdentyfikacyjne.Nazwa);

                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot2?.Adres?.AdresL1))
                    parts.Add(parsedXml.Podmiot2.Adres.AdresL1);

                if (!string.IsNullOrWhiteSpace(parsedXml.Podmiot2?.Adres?.AdresL2))
                    parts.Add(parsedXml.Podmiot2.Adres.AdresL2);

                // Pozycje faktury
                if (parsedXml.Fa?.FaWiersze != null)
                {
                    foreach (var wiersz in parsedXml.Fa.FaWiersze)
                    {
                        if (!string.IsNullOrWhiteSpace(wiersz.P_7))
                            parts.Add(wiersz.P_7); // Nazwa towaru/usługi

                        if (!string.IsNullOrWhiteSpace(wiersz.PKWiU))
                            parts.Add(wiersz.PKWiU);

                        if (!string.IsNullOrWhiteSpace(wiersz.CN))
                            parts.Add(wiersz.CN);

                        if (!string.IsNullOrWhiteSpace(wiersz.GTU))
                            parts.Add(wiersz.GTU);
                    }
                }

                // Uwagi i stopka
                if (!string.IsNullOrWhiteSpace(parsedXml?.Stopka?.Informacje?.StopkaFaktury))
                    parts.Add(parsedXml.Stopka.Informacje.StopkaFaktury);
            }
        }

        // Komentarz
        if (!string.IsNullOrWhiteSpace(invoice.Comment))
            parts.Add(invoice.Comment);

        return string.Join(" ", parts);
    }
}
