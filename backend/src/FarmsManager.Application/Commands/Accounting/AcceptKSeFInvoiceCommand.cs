using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.Sales.Invoices;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

/// <summary>
/// Command do akceptacji faktury KSeF i utworzenia encji modułowej.
/// Zmienia status faktury na "Accepted" i tworzy encję w odpowiednim module.
/// </summary>
public record AcceptKSeFInvoiceCommand : IRequest<BaseResponse<Guid?>>
{
    public Guid InvoiceId { get; init; }
    public ModuleType ModuleType { get; init; }
    
    public CreateFeedInvoiceFromKSeFDto FeedData { get; init; }
    public CreateGasDeliveryFromKSeFDto GasData { get; init; }
    public CreateExpenseProductionFromKSeFDto ExpenseData { get; init; }
    public CreateSaleInvoiceFromKSeFDto SaleData { get; init; }
}

public class AcceptKSeFInvoiceCommandHandler 
    : IRequestHandler<AcceptKSeFInvoiceCommand, BaseResponse<Guid?>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IInvoiceAuditService _auditService;

    public AcceptKSeFInvoiceCommandHandler(
        IUserDataResolver userDataResolver,
        IKSeFInvoiceRepository ksefInvoiceRepository,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IFeedNameRepository feedNameRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IFeedPriceRepository feedPriceRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IGasContractorRepository gasContractorRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IExpenseContractorRepository expenseContractorRepository,
        ISaleInvoiceRepository saleInvoiceRepository,
        ISlaughterhouseRepository slaughterhouseRepository,
        IInvoiceAuditService auditService)
    {
        _userDataResolver = userDataResolver;
        _ksefInvoiceRepository = ksefInvoiceRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _feedNameRepository = feedNameRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedPriceRepository = feedPriceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _gasContractorRepository = gasContractorRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _expenseContractorRepository = expenseContractorRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _slaughterhouseRepository = slaughterhouseRepository;
        _auditService = auditService;
    }

    public async Task<BaseResponse<Guid?>> Handle(AcceptKSeFInvoiceCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var userName = _userDataResolver.GetLoginAsync();
        
        var invoice = await _ksefInvoiceRepository.GetAsync(
            new KSeFInvoiceByIdSpec(request.InvoiceId), ct);

        var previousStatus = invoice.Status;

        // Sprawdź czy faktura nie została już zaakceptowana
        if (invoice.Status == KSeFInvoiceStatus.Accepted)
        {
            throw DomainException.BadRequest("Faktura została już zaakceptowana.");
        }

        // Sprawdź czy faktura nie została przekazana do biura
        if (invoice.Status == KSeFInvoiceStatus.SentToOffice)
        {
            throw DomainException.BadRequest("Faktura została już przekazana do biura.");
        }

        var previousModuleType = invoice.ModuleType;
        var previousEntityId = invoice.AssignedEntityInvoiceId;

        Guid? newEntityId = null;

        // Utwórz encję modułową tylko dla modułów które tego wymagają
        switch (request.ModuleType)
        {
            case ModuleType.Feeds:
                newEntityId = await CreateFeedInvoice(request.FeedData, userId, ct);
                break;
            case ModuleType.Gas:
                newEntityId = await CreateGasDelivery(request.GasData, userId, ct);
                break;
            case ModuleType.ProductionExpenses:
                newEntityId = await CreateExpenseProduction(request.ExpenseData, userId, ct);
                break;
            case ModuleType.Sales:
                newEntityId = await CreateSaleInvoice(request.SaleData, userId, ct);
                break;
            case ModuleType.Farmstead:
            case ModuleType.Other:
            case ModuleType.None:
                // Dla tych modułów nie tworzymy encji modułowej
                break;
        }

        // Jeśli zmienił się moduł i istniała poprzednia encja - usuń ją
        if (previousEntityId.HasValue && previousModuleType != request.ModuleType)
        {
            await DeletePreviousModuleEntity(previousModuleType, previousEntityId.Value, ct);
        }

        // Aktualizuj fakturę: status na Accepted, moduł i encję modułową
        invoice.Update(
            status: KSeFInvoiceStatus.Accepted,
            moduleType: request.ModuleType);
        invoice.SetAssignedEntityInvoiceId(newEntityId);
        invoice.SetModified(userId);
        
        await _ksefInvoiceRepository.UpdateAsync(invoice, ct);

        // Loguj akcję audytową
        await _auditService.LogStatusChangeAsync(
            invoice.Id,
            KSeFInvoiceAuditAction.Accepted,
            previousStatus,
            KSeFInvoiceStatus.Accepted,
            userId,
            userName,
            cancellationToken: ct);

        return BaseResponse.CreateResponse(newEntityId);
    }

    private async Task<Guid> CreateFeedInvoice(CreateFeedInvoiceFromKSeFDto data, Guid userId, CancellationToken ct)
    {
        if (data == null)
            throw DomainException.BadRequest("Dane dostawy paszy są wymagane dla modułu Pasze");

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(data.CycleId), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(data.HenhouseId), ct);

        var feedName = await _feedNameRepository.FirstOrDefaultAsync(
            new GetFeedNameByNameSpec(data.ItemName), ct);
        
        if (feedName is null)
        {
            throw DomainException.BadRequest("Nie znaleziono nazwy paszy w słowniku");
        }

        var newFeedInvoice = FeedInvoiceEntity.CreateNew(
            farm.Id,
            cycle.Id,
            henhouse.Id,
            data.InvoiceNumber,
            data.BankAccountNumber ?? "",
            data.VendorName,
            data.ItemName,
            data.Quantity,
            data.UnitPrice,
            data.InvoiceDate,
            data.DueDate,
            data.InvoiceTotal,
            data.SubTotal,
            data.VatAmount,
            data.Comment,
            userId);

        var feedPrices = await _feedPriceRepository.GetFeedPricesForInvoiceDateAsync(
            farm.Id, cycle.Id, data.ItemName, data.InvoiceDate);
        newFeedInvoice.CheckUnitPrice(feedPrices);

        await _feedInvoiceRepository.AddAsync(newFeedInvoice, ct);
        return newFeedInvoice.Id;
    }

    private async Task<Guid> CreateGasDelivery(CreateGasDeliveryFromKSeFDto data, Guid userId, CancellationToken ct)
    {
        if (data == null)
            throw DomainException.BadRequest("Dane dostawy gazu są wymagane dla modułu Gaz");

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        
        Guid contractorId;
        if (data.ContractorId.HasValue)
        {
            contractorId = data.ContractorId.Value;
        }
        else
        {
            contractorId = await FindOrCreateGasContractor(data.ContractorNip, data.ContractorName, userId, ct);
        }

        var newGasDelivery = GasDeliveryEntity.CreateNew(
            farm.Id,
            contractorId,
            data.InvoiceNumber,
            data.InvoiceDate,
            data.InvoiceTotal,
            data.UnitPrice,
            data.Quantity,
            data.Comment,
            userId);

        await _gasDeliveryRepository.AddAsync(newGasDelivery, ct);
        return newGasDelivery.Id;
    }

    private async Task<Guid> FindOrCreateGasContractor(string nip, string name, Guid userId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(nip))
        {
            var normalizedNip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
            var contractor = await _gasContractorRepository.FirstOrDefaultAsync(
                new GasContractorByNipSpec(normalizedNip), ct);
            if (contractor != null)
                return contractor.Id;
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var contractor = await _gasContractorRepository.FirstOrDefaultAsync(
                new GasContractorByNameSpec(name), ct);
            if (contractor != null)
                return contractor.Id;
        }

        var newContractor = GasContractorEntity.CreateNew(
            name ?? "Nieznany kontrahent",
            nip ?? "",
            "",
            userId);
        
        await _gasContractorRepository.AddAsync(newContractor, ct);
        return newContractor.Id;
    }

    private async Task<Guid> CreateExpenseProduction(CreateExpenseProductionFromKSeFDto data, Guid userId, CancellationToken ct)
    {
        if (data == null)
            throw DomainException.BadRequest("Dane kosztu produkcji są wymagane dla modułu Koszty produkcji");

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(data.CycleId), ct);

        Guid contractorId;
        if (data.ExpenseContractorId.HasValue)
        {
            contractorId = data.ExpenseContractorId.Value;
        }
        else
        {
            contractorId = await FindOrCreateExpenseContractor(data.ContractorNip, data.ContractorName, data.ExpenseTypeId, userId, ct);
        }

        var newExpenseProduction = ExpenseProductionEntity.CreateNew(
            farm.Id,
            cycle.Id,
            contractorId,
            data.ExpenseTypeId,
            data.InvoiceNumber,
            data.InvoiceTotal,
            data.SubTotal,
            data.VatAmount,
            data.InvoiceDate,
            data.Comment,
            userId);

        await _expenseProductionRepository.AddAsync(newExpenseProduction, ct);
        return newExpenseProduction.Id;
    }

    private async Task<Guid> FindOrCreateExpenseContractor(string nip, string name, Guid expenseTypeId, Guid userId, CancellationToken ct)
    {
        ExpenseContractorEntity existingContractor = null;
        
        if (!string.IsNullOrWhiteSpace(nip))
        {
            var normalizedNip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
            existingContractor = await _expenseContractorRepository.FirstOrDefaultAsync(
                new ExpenseContractorByNipWithExpenseTypesSpec(normalizedNip), ct);
        }

        if (existingContractor == null && !string.IsNullOrWhiteSpace(name))
        {
            existingContractor = await _expenseContractorRepository.FirstOrDefaultAsync(
                new ExpenseContractorByNameWithExpenseTypesSpec(name), ct);
        }

        if (existingContractor != null)
        {
            // Kontrahent istnieje - dodaj typ wydatku jeśli go nie ma
            existingContractor.AddExpenseType(expenseTypeId, userId);
            await _expenseContractorRepository.UpdateAsync(existingContractor, ct);
            return existingContractor.Id;
        }

        // Tworzymy nowego kontrahenta z przypisanym typem wydatku
        var newContractor = ExpenseContractorEntity.CreateNew(
            name ?? "Nieznany kontrahent",
            nip ?? "",
            "",
            new[] { expenseTypeId },
            userId);
        
        await _expenseContractorRepository.AddAsync(newContractor, ct);
        return newContractor.Id;
    }

    private async Task<Guid> CreateSaleInvoice(CreateSaleInvoiceFromKSeFDto data, Guid userId, CancellationToken ct)
    {
        if (data == null)
            throw DomainException.BadRequest("Dane faktury sprzedaży są wymagane dla modułu Sprzedaż");

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(data.CycleId), ct);

        Guid slaughterhouseId;
        if (data.SlaughterhouseId.HasValue)
        {
            slaughterhouseId = data.SlaughterhouseId.Value;
        }
        else
        {
            slaughterhouseId = await FindSlaughterhouse(data.SlaughterhouseNip, data.SlaughterhouseName, ct);
        }

        var newSaleInvoice = SaleInvoiceEntity.CreateNew(
            farm.Id,
            cycle.Id,
            slaughterhouseId,
            data.InvoiceNumber,
            data.InvoiceDate,
            data.DueDate,
            data.InvoiceTotal,
            data.SubTotal,
            data.VatAmount,
            userId);

        await _saleInvoiceRepository.AddAsync(newSaleInvoice, ct);
        return newSaleInvoice.Id;
    }

    private async Task<Guid> FindSlaughterhouse(string nip, string name, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(nip))
        {
            var normalizedNip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
            var slaughterhouse = await _slaughterhouseRepository.FirstOrDefaultAsync(
                new SlaughterhouseByNipSpec(normalizedNip), ct);
            if (slaughterhouse != null)
                return slaughterhouse.Id;
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var slaughterhouse = await _slaughterhouseRepository.FirstOrDefaultAsync(
                new SlaughterhouseByNameSpec(name), ct);
            if (slaughterhouse != null)
                return slaughterhouse.Id;
        }

        throw DomainException.BadRequest("Nie znaleziono ubojni. Proszę najpierw dodać ubojnię do systemu.");
    }

    private async Task DeletePreviousModuleEntity(ModuleType moduleType, Guid entityId, CancellationToken ct)
    {
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.FirstOrDefaultAsync(
                    new GetFeedInvoiceByIdSpec(entityId), ct);
                if (feedInvoice != null)
                    await _feedInvoiceRepository.DeleteAsync(feedInvoice, ct);
                break;
            case ModuleType.Gas:
                var gasDelivery = await _gasDeliveryRepository.FirstOrDefaultAsync(
                    new GetGasDeliveryByIdSpec(entityId), ct);
                if (gasDelivery != null)
                    await _gasDeliveryRepository.DeleteAsync(gasDelivery, ct);
                break;
            case ModuleType.ProductionExpenses:
                var expenseProduction = await _expenseProductionRepository.FirstOrDefaultAsync(
                    new GetExpenseProductionByIdSpec(entityId), ct);
                if (expenseProduction != null)
                    await _expenseProductionRepository.DeleteAsync(expenseProduction, ct);
                break;
            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                    new SaleInvoiceByIdSpec(entityId), ct);
                if (saleInvoice != null)
                    await _saleInvoiceRepository.DeleteAsync(saleInvoice, ct);
                break;
        }
    }
}

public class AcceptKSeFInvoiceCommandValidator : AbstractValidator<AcceptKSeFInvoiceCommand>
{
    public AcceptKSeFInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.ModuleType).IsInEnum();

        When(x => x.ModuleType == ModuleType.Feeds, () =>
        {
            RuleFor(x => x.FeedData).NotNull().WithMessage("Dane dostawy paszy są wymagane");
            RuleFor(x => x.FeedData.FarmId).NotEmpty().When(x => x.FeedData != null);
            RuleFor(x => x.FeedData.CycleId).NotEmpty().When(x => x.FeedData != null);
            RuleFor(x => x.FeedData.HenhouseId).NotEmpty().When(x => x.FeedData != null);
            RuleFor(x => x.FeedData.InvoiceNumber).NotEmpty().When(x => x.FeedData != null);
            RuleFor(x => x.FeedData.VendorName).NotEmpty().When(x => x.FeedData != null);
            RuleFor(x => x.FeedData.ItemName).NotEmpty().When(x => x.FeedData != null);
        });

        When(x => x.ModuleType == ModuleType.Gas, () =>
        {
            RuleFor(x => x.GasData).NotNull().WithMessage("Dane dostawy gazu są wymagane");
            RuleFor(x => x.GasData.FarmId).NotEmpty().When(x => x.GasData != null);
            RuleFor(x => x.GasData.InvoiceNumber).NotEmpty().When(x => x.GasData != null);
            RuleFor(x => x.GasData.UnitPrice).GreaterThan(0).When(x => x.GasData != null);
            RuleFor(x => x.GasData.Quantity).GreaterThan(0).When(x => x.GasData != null);
        });

        When(x => x.ModuleType == ModuleType.ProductionExpenses, () =>
        {
            RuleFor(x => x.ExpenseData).NotNull().WithMessage("Dane kosztu produkcji są wymagane");
            RuleFor(x => x.ExpenseData.FarmId).NotEmpty().When(x => x.ExpenseData != null);
            RuleFor(x => x.ExpenseData.CycleId).NotEmpty().When(x => x.ExpenseData != null);
            RuleFor(x => x.ExpenseData.InvoiceNumber).NotEmpty().When(x => x.ExpenseData != null);
        });

        When(x => x.ModuleType == ModuleType.Sales, () =>
        {
            RuleFor(x => x.SaleData).NotNull().WithMessage("Dane faktury sprzedaży są wymagane");
            RuleFor(x => x.SaleData.FarmId).NotEmpty().When(x => x.SaleData != null);
            RuleFor(x => x.SaleData.CycleId).NotEmpty().When(x => x.SaleData != null);
            RuleFor(x => x.SaleData.InvoiceNumber).NotEmpty().When(x => x.SaleData != null);
        });
    }
}
