using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
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

#region Request

public class CreateModuleEntityFromInvoiceRequest
{
    public ModuleType ModuleType { get; set; }
    public CreateFeedInvoiceFromKSeFDto FeedData { get; set; }
    public CreateGasDeliveryFromKSeFDto GasData { get; set; }
    public CreateExpenseProductionFromKSeFDto ExpenseData { get; set; }
    public CreateSaleInvoiceFromKSeFDto SaleData { get; set; }
}

#endregion

#region DTOs

public record CreateFeedInvoiceFromKSeFDto
{
    public Guid InvoiceId { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }
    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public string VendorName { get; init; }
    public string ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public string Comment { get; init; }
}

public record CreateGasDeliveryFromKSeFDto
{
    public Guid InvoiceId { get; init; }
    public Guid FarmId { get; init; }
    public Guid? ContractorId { get; init; }
    public string ContractorNip { get; init; }
    public string ContractorName { get; init; }
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public string Comment { get; init; }
}

public record CreateExpenseProductionFromKSeFDto
{
    public Guid InvoiceId { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid? ExpenseContractorId { get; init; }
    public Guid ExpenseTypeId { get; init; }
    public string ContractorNip { get; init; }
    public string ContractorName { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public string Comment { get; init; }
}

public record CreateSaleInvoiceFromKSeFDto
{
    public Guid InvoiceId { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid? SlaughterhouseId { get; init; }
    public string SlaughterhouseNip { get; init; }
    public string SlaughterhouseName { get; init; }
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
}

#endregion

public record CreateModuleEntityFromInvoiceCommand : IRequest<BaseResponse<Guid?>>
{
    public Guid KSeFInvoiceId { get; init; }
    public ModuleType ModuleType { get; init; }
    
    public CreateFeedInvoiceFromKSeFDto FeedData { get; init; }
    public CreateGasDeliveryFromKSeFDto GasData { get; init; }
    public CreateExpenseProductionFromKSeFDto ExpenseData { get; init; }
    public CreateSaleInvoiceFromKSeFDto SaleData { get; init; }
}

public class CreateModuleEntityFromInvoiceCommandHandler 
    : IRequestHandler<CreateModuleEntityFromInvoiceCommand, BaseResponse<Guid?>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;
    private readonly IS3Service _s3Service;
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

    public CreateModuleEntityFromInvoiceCommandHandler(
        IUserDataResolver userDataResolver,
        IKSeFInvoiceRepository ksefInvoiceRepository,
        IS3Service s3Service,
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
        ISlaughterhouseRepository slaughterhouseRepository)
    {
        _userDataResolver = userDataResolver;
        _ksefInvoiceRepository = ksefInvoiceRepository;
        _s3Service = s3Service;
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
    }

    public async Task<BaseResponse<Guid?>> Handle(CreateModuleEntityFromInvoiceCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var invoice = await _ksefInvoiceRepository.GetAsync(
            new KSeFInvoiceByIdSpec(request.KSeFInvoiceId), ct);

        var previousModuleType = invoice.ModuleType;
        var previousEntityId = invoice.AssignedEntityInvoiceId;

        Guid? newEntityId = null;
        var invoiceFilePath = invoice.FilePath;

        switch (request.ModuleType)
        {
            case ModuleType.Feeds:
                newEntityId = await CreateFeedInvoice(request.FeedData, invoiceFilePath, userId, ct);
                break;
            case ModuleType.Gas:
                newEntityId = await CreateGasDelivery(request.GasData, invoiceFilePath, userId, ct);
                break;
            case ModuleType.ProductionExpenses:
                newEntityId = await CreateExpenseProduction(request.ExpenseData, invoiceFilePath, userId, ct);
                break;
            case ModuleType.Sales:
                newEntityId = await CreateSaleInvoice(request.SaleData, invoiceFilePath, userId, ct);
                break;
            case ModuleType.Farmstead:
            case ModuleType.Other:
            case ModuleType.None:
                break;
        }

        if (previousEntityId.HasValue && previousModuleType != request.ModuleType)
        {
            await DeletePreviousModuleEntity(previousModuleType, previousEntityId.Value, ct);
        }

        invoice.Update(moduleType: request.ModuleType);
        invoice.SetAssignedEntityInvoiceId(newEntityId);
        invoice.SetModified(userId);
        
        await _ksefInvoiceRepository.UpdateAsync(invoice, ct);

        return BaseResponse.CreateResponse(newEntityId);
    }

    private async Task<Guid> CreateFeedInvoice(CreateFeedInvoiceFromKSeFDto data, string filePath, Guid userId, CancellationToken ct)
    {
        var exists = await _feedInvoiceRepository.AnyAsync(
            new GetFeedInvoiceByInvoiceNumberSpec(data.InvoiceNumber), ct);

        if (exists)
        {
            throw DomainException.BadRequest($"Faktura paszowa o numerze '{data.InvoiceNumber}' już istnieje.");
        }

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

        // Skopiuj plik z faktury księgowej do folderu modułowego
        if (!string.IsNullOrEmpty(filePath))
        {
            var copiedFilePath = await CopyFileToModuleAsync(filePath, FileType.FeedDeliveryInvoice, newFeedInvoice.Id, ct);
            if (!string.IsNullOrEmpty(copiedFilePath))
            {
                newFeedInvoice.SetFilePath(copiedFilePath);
            }
        }

        var feedPrices = await _feedPriceRepository.GetFeedPricesForInvoiceDateAsync(
            farm.Id, cycle.Id, data.ItemName, data.InvoiceDate);
        newFeedInvoice.CheckUnitPrice(feedPrices);

        await _feedInvoiceRepository.AddAsync(newFeedInvoice, ct);
        return newFeedInvoice.Id;
    }

    private async Task<Guid> CreateGasDelivery(CreateGasDeliveryFromKSeFDto data, string filePath, Guid userId, CancellationToken ct)
    {
        var exists = await _gasDeliveryRepository.AnyAsync(
            new GetGasDeliveryByInvoiceNumberSpec(data.InvoiceNumber), ct);

        if (exists)
        {
            throw DomainException.BadRequest($"Dostawa gazu o numerze faktury '{data.InvoiceNumber}' już istnieje.");
        }

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        
        Guid contractorId;
        if (data.ContractorId.HasValue)
        {
            contractorId = data.ContractorId.Value;
        }
        else
        {
            var contractor = await FindOrCreateGasContractor(data.ContractorNip, data.ContractorName, userId, ct);
            contractorId = contractor;
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

        // Skopiuj plik z faktury księgowej do folderu modułowego
        if (!string.IsNullOrEmpty(filePath))
        {
            var copiedFilePath = await CopyFileToModuleAsync(filePath, FileType.GasDelivery, newGasDelivery.Id, ct);
            if (!string.IsNullOrEmpty(copiedFilePath))
            {
                newGasDelivery.SetFilePath(copiedFilePath);
            }
        }

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

    private async Task<Guid> CreateExpenseProduction(CreateExpenseProductionFromKSeFDto data, string filePath, Guid userId, CancellationToken ct)
    {
        var exists = await _expenseProductionRepository.AnyAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(data.InvoiceNumber), ct);

        if (exists)
        {
            throw DomainException.BadRequest($"Koszt produkcyjny o numerze faktury '{data.InvoiceNumber}' już istnieje.");
        }

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(data.CycleId), ct);

        Guid contractorId;
        if (data.ExpenseContractorId.HasValue)
        {
            contractorId = data.ExpenseContractorId.Value;
        }
        else
        {
            var contractor = await FindOrCreateExpenseContractor(data.ContractorNip, data.ContractorName, userId, ct);
            contractorId = contractor;
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

        // Skopiuj plik z faktury księgowej do folderu modułowego
        if (!string.IsNullOrEmpty(filePath))
        {
            var copiedFilePath = await CopyFileToModuleAsync(filePath, FileType.ExpenseProduction, newExpenseProduction.Id, ct);
            if (!string.IsNullOrEmpty(copiedFilePath))
            {
                newExpenseProduction.SetFilePath(copiedFilePath);
            }
        }

        await _expenseProductionRepository.AddAsync(newExpenseProduction, ct);
        return newExpenseProduction.Id;
    }

    private async Task<Guid> FindOrCreateExpenseContractor(string nip, string name, Guid userId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(nip))
        {
            var normalizedNip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
            var contractor = await _expenseContractorRepository.FirstOrDefaultAsync(
                new ExpenseContractorByNipSpec(normalizedNip), ct);
            if (contractor != null)
                return contractor.Id;
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var contractor = await _expenseContractorRepository.FirstOrDefaultAsync(
                new ExpenseContractorByNameSpec(name), ct);
            if (contractor != null)
                return contractor.Id;
        }

        var newContractor = ExpenseContractorEntity.CreateNewFromInvoice(
            name ?? "Nieznany kontrahent",
            nip ?? "",
            "",
            userId);
        
        await _expenseContractorRepository.AddAsync(newContractor, ct);
        return newContractor.Id;
    }

    private async Task<Guid> CreateSaleInvoice(CreateSaleInvoiceFromKSeFDto data, string filePath, Guid userId, CancellationToken ct)
    {
        var exists = await _saleInvoiceRepository.AnyAsync(
            new GetSaleInvoiceByInvoiceNumberSpec(data.InvoiceNumber), ct);

        if (exists)
        {
            throw DomainException.BadRequest($"Faktura sprzedaży o numerze '{data.InvoiceNumber}' już istnieje.");
        }

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(data.CycleId), ct);

        Guid slaughterhouseId;
        if (data.SlaughterhouseId.HasValue)
        {
            slaughterhouseId = data.SlaughterhouseId.Value;
        }
        else
        {
            var slaughterhouse = await FindSlaughterhouse(data.SlaughterhouseNip, data.SlaughterhouseName, ct);
            slaughterhouseId = slaughterhouse;
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

        // Skopiuj plik z faktury księgowej do folderu modułowego
        if (!string.IsNullOrEmpty(filePath))
        {
            var copiedFilePath = await CopyFileToModuleAsync(filePath, FileType.SalesInvoices, newSaleInvoice.Id, ct);
            if (!string.IsNullOrEmpty(copiedFilePath))
            {
                newSaleInvoice.SetFilePath(copiedFilePath);
            }
        }

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

    /// <summary>
    /// Kopiuje plik z faktury księgowej do folderu modułowego
    /// </summary>
    private async Task<string> CopyFileToModuleAsync(string sourceFilePath, FileType targetFileType, Guid entityId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(sourceFilePath))
            return null;

        try
        {
            // Pobierz plik źródłowy z folderu accounting
            var sourceFile = await _s3Service.GetFileAsync(FileType.AccountingInvoice, sourceFilePath);
            if (sourceFile?.Data == null || sourceFile.Data.Length == 0)
                return null;

            // Określ rozszerzenie pliku
            var extension = Path.GetExtension(sourceFilePath);
            if (string.IsNullOrEmpty(extension))
                extension = ".pdf";

            // Utwórz ścieżkę docelową w folderze modułowym
            var targetPath = $"{entityId}{extension}";

            // Skopiuj plik do folderu modułowego
            var uploadedPath = await _s3Service.UploadFileAsync(
                sourceFile.Data, 
                targetFileType, 
                targetPath, 
                ct);

            return uploadedPath;
        }
        catch
        {
            // W przypadku błędu kopiowania, zwróć oryginalną ścieżkę
            return sourceFilePath;
        }
    }
}

public class CreateModuleEntityFromInvoiceCommandValidator : AbstractValidator<CreateModuleEntityFromInvoiceCommand>
{
    public CreateModuleEntityFromInvoiceCommandValidator()
    {
        RuleFor(x => x.KSeFInvoiceId).NotEmpty();
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
