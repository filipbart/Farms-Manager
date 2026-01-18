using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

public class ContractorAutoCreationService : IContractorAutoCreationService
{
    private readonly IFeedContractorRepository _feedContractorRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly ILogger<ContractorAutoCreationService> _logger;

    public ContractorAutoCreationService(
        IFeedContractorRepository feedContractorRepository,
        IExpenseContractorRepository expenseContractorRepository,
        IGasContractorRepository gasContractorRepository,
        ILogger<ContractorAutoCreationService> logger)
    {
        _feedContractorRepository = feedContractorRepository;
        _expenseContractorRepository = expenseContractorRepository;
        _gasContractorRepository = gasContractorRepository;
        _logger = logger;
    }

    public async Task<Guid?> EnsureContractorExistsAsync(
        string nip,
        string name,
        string address,
        ModuleType moduleType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nip))
            return null;

        var cleanNip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();

        switch (moduleType)
        {
            case ModuleType.Feeds:
                return await EnsureFeedContractorAsync(cleanNip, name, cancellationToken);
            case ModuleType.ProductionExpenses:
                return await EnsureExpenseContractorAsync(cleanNip, name, address, cancellationToken);
            case ModuleType.Gas:
                return await EnsureGasContractorAsync(cleanNip, name, cancellationToken);
            default:
                return null;
        }
    }

    private async Task<Guid?> EnsureFeedContractorAsync(string nip, string name, CancellationToken cancellationToken)
    {
        var existing = await _feedContractorRepository.FirstOrDefaultAsync(
            new FeedContractorByNipSpec(nip), cancellationToken);

        if (existing != null)
            return existing.Id;

        var contractor = FeedContractorEntity.CreateNewFromInvoice(name, nip);
        await _feedContractorRepository.AddAsync(contractor, cancellationToken);
        await _feedContractorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Auto-created FeedContractor: {Name} ({Nip})", name, nip);
        return contractor.Id;
    }

    private async Task<Guid?> EnsureExpenseContractorAsync(string nip, string name, string address, CancellationToken cancellationToken)
    {
        var existing = await _expenseContractorRepository.FirstOrDefaultAsync(
            new ExpenseContractorByNipSpec(nip), cancellationToken);

        if (existing != null)
            return existing.Id;

        var contractor = ExpenseContractorEntity.CreateNewFromInvoice(name, nip, address ?? string.Empty);
        await _expenseContractorRepository.AddAsync(contractor, cancellationToken);
        await _expenseContractorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Auto-created ExpenseContractor: {Name} ({Nip})", name, nip);
        return contractor.Id;
    }

    private async Task<Guid?> EnsureGasContractorAsync(string nip, string name, CancellationToken cancellationToken)
    {
        var existing = await _gasContractorRepository.FirstOrDefaultAsync(
            new GasContractorByNipSpec(nip), cancellationToken);

        if (existing != null)
            return existing.Id;

        var contractor = GasContractorEntity.CreateNew(name, nip, string.Empty);
        await _gasContractorRepository.AddAsync(contractor, cancellationToken);
        await _gasContractorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Auto-created GasContractor: {Name} ({Nip})", name, nip);
        return contractor.Id;
    }
}
