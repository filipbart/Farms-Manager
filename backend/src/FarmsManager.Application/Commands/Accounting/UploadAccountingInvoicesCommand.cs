using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Application.Specifications.TaxBusinessEntities;
using FarmsManager.Application.Commands.Sales.Invoices;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Accounting;

public record UploadAccountingInvoicesCommandDto
{
    public List<IFormFile> Files { get; init; }
    public string InvoiceType { get; init; } // Purchase or Sales
    public string PaymentStatus { get; init; } // Unpaid, PaidCash, PaidTransfer, etc.
    public string ModuleType { get; init; } // Optional: Target module for AI extraction steering
    public DateOnly? PaymentDate { get; init; } // Optional: Payment date when status is paid
}

/// <summary>
/// Dane zaczytane z faktury przez AI
/// </summary>
public record AccountingInvoiceExtractedData
{
    public string InvoiceNumber { get; init; }
    public string InvoiceDate { get; init; }
    public string DueDate { get; init; }
    public string SellerName { get; init; }
    public string SellerNip { get; init; }
    public string SellerAddress { get; init; }
    public string BuyerName { get; init; }
    public string BuyerNip { get; init; }
    public string BuyerAddress { get; init; }
    public decimal? GrossAmount { get; init; }
    public decimal? NetAmount { get; init; }
    public decimal? VatAmount { get; init; }
    public string BankAccountNumber { get; init; }
    public string InvoiceType { get; init; }
    public string PaymentStatus { get; init; }
    public string PaymentDate { get; init; }
    public Guid? FarmId { get; init; }
    public Guid? CycleId { get; init; }
    public string ModuleType { get; init; }
    
    // Module-specific fields
    public Guid? FeedContractorId { get; init; }
    public Guid? GasContractorId { get; init; }
    public Guid? ExpenseContractorId { get; init; }
    public Guid? SlaughterhouseId { get; init; }
    public Guid? HenhouseId { get; init; }
    public string HenhouseName { get; init; }
    
    // Flags for new entities created during upload
    public bool IsNewFeedContractor { get; init; }
    public bool IsNewGasContractor { get; init; }
    public bool IsNewExpenseContractor { get; init; }
    public bool IsNewSlaughterhouse { get; init; }
}

public record UploadAccountingInvoiceFileData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AccountingInvoiceExtractedData ExtractedFields { get; init; }
}

public record UploadAccountingInvoicesCommandResponse
{
    public List<UploadAccountingInvoiceFileData> Files { get; set; } = [];
}

public record UploadAccountingInvoicesCommand(UploadAccountingInvoicesCommandDto Data)
    : IRequest<BaseResponse<UploadAccountingInvoicesCommandResponse>>;

public class UploadAccountingInvoicesCommandHandler : IRequestHandler<UploadAccountingInvoicesCommand,
    BaseResponse<UploadAccountingInvoicesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedContractorRepository _feedContractorRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly ITaxBusinessEntityRepository _taxBusinessEntityRepository;

    public UploadAccountingInvoicesCommandHandler(
        IMapper mapper,
        IS3Service s3Service,
        IAzureDiService azureDiService,
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        IHenhouseRepository henhouseRepository,
        IFeedContractorRepository feedContractorRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasContractorRepository gasContractorRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IExpenseContractorRepository expenseContractorRepository,
        IExpenseProductionRepository expenseProductionRepository,
        ISlaughterhouseRepository slaughterhouseRepository,
        ISaleInvoiceRepository saleInvoiceRepository,
        ITaxBusinessEntityRepository taxBusinessEntityRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
        _feedContractorRepository = feedContractorRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasContractorRepository = gasContractorRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _expenseContractorRepository = expenseContractorRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _slaughterhouseRepository = slaughterhouseRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _taxBusinessEntityRepository = taxBusinessEntityRepository;
    }

    public async Task<BaseResponse<UploadAccountingInvoicesCommandResponse>> Handle(
        UploadAccountingInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new UploadAccountingInvoicesCommandResponse();

        foreach (var file in request.Data.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileId + extension;
            
            // Determine FileType based on module type
            var fileType = GetFileTypeForModule(request.Data.ModuleType);
            var filePath = $"draft/{fileId}{extension}";

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, fileType, filePath,
                cancellationToken, contentType: file.ContentType);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(fileType, filePath, newFileName);

            // Zaczytaj dane z faktury przez AI - użyj odpowiedniego modelu w zależności od ModuleType
            var extractedFields = await AnalyzeInvoiceByModuleTypeAsync(
                preSignedUrl, request.Data.ModuleType, cancellationToken);
            
            // Próba automatycznego dopasowania fermy na podstawie podmiotu gospodarczego
            // Dla Sales: ferma = Seller (my sprzedajemy), dla pozostałych: ferma = Buyer (my kupujemy)
            var isSalesModule = request.Data.ModuleType == nameof(ModuleType.Sales);
            var matchedFarm = await MatchFarmEntityAsync(
                extractedFields.SellerNip, extractedFields.SellerName, 
                extractedFields.BuyerNip, extractedFields.BuyerName, 
                isSalesModule, cancellationToken);

            // Przetwórz moduł-specyficzne dane (kontrahenci, duplikaty, itp.)
            var moduleData = await ProcessModuleSpecificDataAsync(
                request.Data.ModuleType, extractedFields, matchedFarm, userId, cancellationToken);

            extractedFields = extractedFields with 
            { 
                InvoiceType = request.Data.InvoiceType,
                PaymentStatus = request.Data.PaymentStatus,
                PaymentDate = request.Data.PaymentDate?.ToString("yyyy-MM-dd"),
                FarmId = matchedFarm?.Id,
                CycleId = matchedFarm?.ActiveCycleId,
                ModuleType = request.Data.ModuleType,
                FeedContractorId = moduleData.FeedContractorId,
                GasContractorId = moduleData.GasContractorId,
                ExpenseContractorId = moduleData.ExpenseContractorId,
                SlaughterhouseId = moduleData.SlaughterhouseId,
                HenhouseId = moduleData.HenhouseId,
                HenhouseName = moduleData.HenhouseName,
                IsNewFeedContractor = moduleData.IsNewFeedContractor,
                IsNewGasContractor = moduleData.IsNewGasContractor,
                IsNewExpenseContractor = moduleData.IsNewExpenseContractor,
                IsNewSlaughterhouse = moduleData.IsNewSlaughterhouse
            };

            response.Files.Add(new UploadAccountingInvoiceFileData
            {
                DraftId = fileId,
                FilePath = key,
                FileUrl = preSignedUrl,
                ExtractedFields = extractedFields
            });
        }

        return BaseResponse.CreateResponse(response);
    }

    private record ModuleSpecificData(
        Guid? FeedContractorId = null,
        Guid? GasContractorId = null,
        Guid? ExpenseContractorId = null,
        Guid? SlaughterhouseId = null,
        Guid? HenhouseId = null,
        string HenhouseName = null,
        bool IsNewFeedContractor = false,
        bool IsNewGasContractor = false,
        bool IsNewExpenseContractor = false,
        bool IsNewSlaughterhouse = false);

    private async Task<ModuleSpecificData> ProcessModuleSpecificDataAsync(
        string moduleTypeStr, AccountingInvoiceExtractedData extractedFields, 
        FarmEntity farm, Guid userId, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ModuleType>(moduleTypeStr, out var moduleType))
            return new ModuleSpecificData();

        return moduleType switch
        {
            ModuleType.Feeds => await ProcessFeedsModuleAsync(extractedFields, farm, userId, cancellationToken),
            ModuleType.Gas => await ProcessGasModuleAsync(extractedFields, userId, cancellationToken),
            ModuleType.ProductionExpenses => await ProcessExpenseModuleAsync(extractedFields, userId, cancellationToken),
            ModuleType.Sales => await ProcessSalesModuleAsync(extractedFields, farm, userId, cancellationToken),
            _ => new ModuleSpecificData()
        };
    }

    private async Task<ModuleSpecificData> ProcessFeedsModuleAsync(
        AccountingInvoiceExtractedData extractedFields, FarmEntity farm, Guid userId, CancellationToken cancellationToken)
    {
        var isNew = false;
        
        // Szukaj/twórz FeedContractor
        var contractor = await _feedContractorRepository.FirstOrDefaultAsync(
            new FeedContractorByNipSpec(extractedFields.SellerNip?.Replace("-", "")), cancellationToken);

        if (contractor is null && extractedFields.SellerNip.IsNotEmpty())
        {
            contractor = FeedContractorEntity.CreateNewFromInvoice(
                extractedFields.SellerName, extractedFields.SellerNip, userId);
            await _feedContractorRepository.AddAsync(contractor, cancellationToken);
            isNew = true;
        }

        // Sprawdź duplikaty
        if (!string.IsNullOrEmpty(extractedFields.InvoiceNumber))
        {
            var existedInvoice = await _feedInvoiceRepository.SingleOrDefaultAsync(
                new GetFeedInvoiceByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw DomainException.BadRequest(
                    $"Istnieje już dostawa paszy z numerem faktury '{existedInvoice.InvoiceNumber}'.");
            }
        }

        // Szukaj kurnika po nazwie (z modelu FeedDeliveryInvoiceModel.HenhouseName)
        Guid? henhouseId = null;
        string henhouseName = null;
        if (farm != null)
        {
            var henhouse = await _henhouseRepository.FirstOrDefaultAsync(
                new HenhouseByNameAndFarmIdSpec(extractedFields.HenhouseName, farm.Id), cancellationToken);
            henhouseId = henhouse?.Id;
            henhouseName = extractedFields.HenhouseName;
        }

        return new ModuleSpecificData(
            FeedContractorId: contractor?.Id,
            HenhouseId: henhouseId,
            HenhouseName: henhouseName,
            IsNewFeedContractor: isNew);
    }

    private async Task<ModuleSpecificData> ProcessGasModuleAsync(
        AccountingInvoiceExtractedData extractedFields, Guid userId, CancellationToken cancellationToken)
    {
        var isNew = false;

        // Szukaj/twórz GasContractor
        var contractor = await _gasContractorRepository.FirstOrDefaultAsync(
            new GasContractorByNipSpec(extractedFields.SellerNip?.Replace("-", "")), cancellationToken);

        if (contractor is null && extractedFields.SellerNip.IsNotEmpty())
        {
            contractor = GasContractorEntity.CreateNew(
                extractedFields.SellerName, extractedFields.SellerNip,
                extractedFields.SellerAddress?.Replace("\n", " "), userId);
            await _gasContractorRepository.AddAsync(contractor, cancellationToken);
            isNew = true;
        }

        // Sprawdź duplikaty
        if (!string.IsNullOrEmpty(extractedFields.InvoiceNumber))
        {
            var existedInvoice = await _gasDeliveryRepository.FirstOrDefaultAsync(
                new GetGasDeliveryByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw DomainException.BadRequest(
                    $"Istnieje już dostawa gazu z numerem faktury '{existedInvoice.InvoiceNumber}'.");
            }
        }

        return new ModuleSpecificData(GasContractorId: contractor?.Id, IsNewGasContractor: isNew);
    }

    private async Task<ModuleSpecificData> ProcessExpenseModuleAsync(
        AccountingInvoiceExtractedData extractedFields, Guid userId, CancellationToken cancellationToken)
    {
        var isNew = false;

        // Szukaj/twórz ExpenseContractor
        var contractor = await _expenseContractorRepository.FirstOrDefaultAsync(
            new ExpenseContractorByNipSpec(extractedFields.SellerNip?.Replace("-", "")), cancellationToken);

        if (contractor is null && extractedFields.SellerNip.IsNotEmpty())
        {
            contractor = ExpenseContractorEntity.CreateNewFromInvoice(
                extractedFields.SellerName, extractedFields.SellerNip ?? string.Empty,
                extractedFields.SellerAddress?.Replace("\n", "") ?? string.Empty, userId);
            await _expenseContractorRepository.AddAsync(contractor, cancellationToken);
            isNew = true;
        }

        // Sprawdź duplikaty (po numerze faktury + kontrahent)
        if (!string.IsNullOrEmpty(extractedFields.InvoiceNumber) && contractor is not null)
        {
            var existedInvoice = await _expenseProductionRepository.FirstOrDefaultAsync(
                new GetExpenseProductionInvoiceByInvoiceNumberAndContractorSpec(extractedFields.InvoiceNumber, contractor.Id),
                cancellationToken);

            if (existedInvoice is not null)
            {
                throw DomainException.BadRequest(
                    $"Istnieje już koszt produkcyjny z numerem faktury '{existedInvoice.InvoiceNumber}' dla sprzedawcy '{contractor.Name}'.");
            }
        }

        return new ModuleSpecificData(ExpenseContractorId: contractor?.Id, IsNewExpenseContractor: isNew);
    }

    private async Task<ModuleSpecificData> ProcessSalesModuleAsync(
        AccountingInvoiceExtractedData extractedFields, FarmEntity farm, Guid userId, CancellationToken cancellationToken)
    {
        var isNew = false;

        // Szukaj/twórz Slaughterhouse (rzeźnia = Customer w fakturze sprzedaży)
        var slaughterhouse = await _slaughterhouseRepository.FirstOrDefaultAsync(
            new SlaughterhouseByNipSpec(extractedFields.BuyerNip?.Replace("-", "")), cancellationToken);

        if (slaughterhouse is null && extractedFields.BuyerNip.IsNotEmpty())
        {
            slaughterhouse = SlaughterhouseEntity.CreateNew(
                extractedFields.BuyerName, string.Empty,
                extractedFields.BuyerNip, extractedFields.BuyerAddress, userId);
            await _slaughterhouseRepository.AddAsync(slaughterhouse, cancellationToken);
            isNew = true;
        }

        // Sprawdź duplikaty (po numerze faktury + ferma)
        if (!string.IsNullOrEmpty(extractedFields.InvoiceNumber) && farm is not null)
        {
            var existedInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                new GetSaleInvoiceByInvoiceNumberAndFarmSpec(extractedFields.InvoiceNumber, farm.Id),
                cancellationToken);

            if (existedInvoice is not null)
            {
                throw DomainException.BadRequest(
                    $"Istnieje już faktura sprzedaży z numerem '{existedInvoice.InvoiceNumber}' dla fermy '{farm.Name}'.");
            }
        }

        return new ModuleSpecificData(SlaughterhouseId: slaughterhouse?.Id, IsNewSlaughterhouse: isNew);
    }

    private async Task<FarmEntity> MatchFarmEntityAsync(
        string sellerNip, string sellerName, string buyerNip, string buyerName,
        bool isSalesModule, CancellationToken cancellationToken)
    {
        FarmEntity farm = null;

        // Dla Sales: ferma = Seller (my sprzedajemy), dla pozostałych: ferma = Buyer (my kupujemy)
        var (primaryNip, primaryName, secondaryNip, secondaryName) = isSalesModule
            ? (sellerNip, sellerName, buyerNip, buyerName)
            : (buyerNip, buyerName, sellerNip, sellerName);

        // 1. Szukaj fermy bezpośrednio po NIP
        if (!string.IsNullOrWhiteSpace(primaryNip))
        {
            farm = await _farmRepository.FirstOrDefaultAsync(
                new FarmByNipSpec(primaryNip), cancellationToken);
        }

        if (farm is null && !string.IsNullOrWhiteSpace(secondaryNip))
        {
            farm = await _farmRepository.FirstOrDefaultAsync(
                new FarmByNipSpec(secondaryNip), cancellationToken);
        }

        // 2. Szukaj po nazwie
        if (farm is null && !string.IsNullOrWhiteSpace(primaryName))
        {
            farm = await _farmRepository.FirstOrDefaultAsync(
                new FarmByNameSpec(primaryName), cancellationToken);
        }

        if (farm is null && !string.IsNullOrWhiteSpace(secondaryName))
        {
            farm = await _farmRepository.FirstOrDefaultAsync(
                new FarmByNameSpec(secondaryName), cancellationToken);
        }

        // 3. Fallback: szukaj przez TaxBusinessEntity
        if (farm is null)
        {
            farm = await MatchFarmViaTaxBusinessEntityAsync(primaryNip, primaryName, secondaryNip, secondaryName, cancellationToken);
        }

        return farm;
    }

    private async Task<FarmEntity> MatchFarmViaTaxBusinessEntityAsync(
        string sellerNip, string sellerName, string buyerNip, string buyerName,
        CancellationToken cancellationToken)
    {
        var taxBusinessEntities = await _taxBusinessEntityRepository.ListAsync(
            new AllActiveTaxBusinessEntitiesSpec(),
            cancellationToken);

        if (taxBusinessEntities.Count == 0)
            return null;

        sellerNip = NormalizeNip(sellerNip);
        buyerNip = NormalizeNip(buyerNip);

        var matchedEntity = taxBusinessEntities.FirstOrDefault(t =>
            (t.Nip != null && (t.Nip == sellerNip || t.Nip == buyerNip)));

        if (matchedEntity == null)
        {
            var sellerNameLower = sellerName?.ToLowerInvariant();
            var buyerNameLower = buyerName?.ToLowerInvariant();

            matchedEntity = taxBusinessEntities.FirstOrDefault(t =>
            {
                var entityName = t.Name?.ToLowerInvariant();
                if (string.IsNullOrEmpty(entityName))
                    return false;

                return (!string.IsNullOrEmpty(sellerNameLower) && sellerNameLower.Contains(entityName)) ||
                       (!string.IsNullOrEmpty(buyerNameLower) && buyerNameLower.Contains(entityName)) ||
                       (!string.IsNullOrEmpty(sellerNameLower) && entityName.Contains(sellerNameLower)) ||
                       (!string.IsNullOrEmpty(buyerNameLower) && entityName.Contains(buyerNameLower));
            });
        }

        // Jeśli znaleziono podmiot i ma on dokładnie jedną fermę, zwróć ją
        if (matchedEntity != null && matchedEntity.Farms.Count == 1)
        {
            return matchedEntity.Farms.First();
        }

        return null;
    }

    private static string NormalizeNip(string nip)
    {
        return nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    }

    /// <summary>
    /// Analizuje fakturę przy użyciu odpowiedniego modelu Azure DI w zależności od typu modułu
    /// </summary>
    private async Task<AccountingInvoiceExtractedData> AnalyzeInvoiceByModuleTypeAsync(
        string preSignedUrl, string moduleTypeStr, CancellationToken cancellationToken)
    {
        // Parsuj ModuleType z stringa
        if (!Enum.TryParse<ModuleType>(moduleTypeStr, out var moduleType))
        {
            moduleType = ModuleType.None;
        }

        return moduleType switch
        {
            ModuleType.Feeds => await AnalyzeFeedInvoiceAsync(preSignedUrl, cancellationToken),
            ModuleType.Gas => await AnalyzeGasInvoiceAsync(preSignedUrl, cancellationToken),
            ModuleType.ProductionExpenses => await AnalyzeExpenseInvoiceAsync(preSignedUrl, cancellationToken),
            ModuleType.Sales => await AnalyzeSaleInvoiceAsync(preSignedUrl, cancellationToken),
            _ => await AnalyzeBasicInvoiceAsync(preSignedUrl, cancellationToken)
        };
    }

    private async Task<AccountingInvoiceExtractedData> AnalyzeBasicInvoiceAsync(
        string preSignedUrl, CancellationToken cancellationToken)
    {
        var model = await _azureDiService.AnalyzeInvoiceAsync<AccountingInvoiceModel>(preSignedUrl, cancellationToken);
        return _mapper.Map<AccountingInvoiceExtractedData>(model);
    }

    private async Task<AccountingInvoiceExtractedData> AnalyzeFeedInvoiceAsync(
        string preSignedUrl, CancellationToken cancellationToken)
    {
        var model = await _azureDiService.AnalyzeInvoiceAsync<FeedDeliveryInvoiceModel>(preSignedUrl, cancellationToken);
        return new AccountingInvoiceExtractedData
        {
            InvoiceNumber = model.InvoiceNumber,
            InvoiceDate = model.InvoiceDate?.ToString("yyyy-MM-dd"),
            DueDate = model.DueDate?.ToString("yyyy-MM-dd"),
            SellerName = model.VendorName,
            SellerNip = model.VendorNip,
            BuyerName = model.CustomerName,
            BuyerNip = model.CustomerNip,
            GrossAmount = model.InvoiceTotal,
            NetAmount = model.SubTotal,
            VatAmount = model.VatAmount,
            BankAccountNumber = model.BankAccountNumber
        };
    }

    private async Task<AccountingInvoiceExtractedData> AnalyzeGasInvoiceAsync(
        string preSignedUrl, CancellationToken cancellationToken)
    {
        var model = await _azureDiService.AnalyzeInvoiceAsync<GasDeliveryInvoiceModel>(preSignedUrl, cancellationToken);
        return new AccountingInvoiceExtractedData
        {
            InvoiceNumber = model.InvoiceNumber,
            InvoiceDate = model.InvoiceDate?.ToString("yyyy-MM-dd"),
            DueDate = model.DueDate?.ToString("yyyy-MM-dd"),
            SellerName = model.VendorName,
            SellerNip = model.VendorNip,
            SellerAddress = model.VendorAddress,
            BuyerName = model.CustomerName,
            BuyerNip = model.CustomerNip,
            GrossAmount = model.InvoiceTotal
        };
    }

    private async Task<AccountingInvoiceExtractedData> AnalyzeExpenseInvoiceAsync(
        string preSignedUrl, CancellationToken cancellationToken)
    {
        var model = await _azureDiService.AnalyzeInvoiceAsync<ExpenseProductionInvoiceModel>(preSignedUrl, cancellationToken);
        return new AccountingInvoiceExtractedData
        {
            InvoiceNumber = model.InvoiceNumber,
            InvoiceDate = model.InvoiceDate?.ToString("yyyy-MM-dd"),
            DueDate = model.DueDate?.ToString("yyyy-MM-dd"),
            SellerName = model.VendorName,
            SellerNip = model.VendorNip,
            SellerAddress = model.VendorAddress,
            BuyerName = model.CustomerName,
            BuyerNip = model.CustomerNip,
            GrossAmount = model.InvoiceTotal,
            NetAmount = model.SubTotal,
            VatAmount = model.VatAmount
        };
    }

    private async Task<AccountingInvoiceExtractedData> AnalyzeSaleInvoiceAsync(
        string preSignedUrl, CancellationToken cancellationToken)
    {
        var model = await _azureDiService.AnalyzeInvoiceAsync<SaleInvoiceModel>(preSignedUrl, cancellationToken);
        return new AccountingInvoiceExtractedData
        {
            InvoiceNumber = model.InvoiceNumber,
            InvoiceDate = model.InvoiceDate?.ToString("yyyy-MM-dd"),
            DueDate = model.DueDate?.ToString("yyyy-MM-dd"),
            SellerName = model.VendorName,
            SellerNip = model.VendorNip,
            BuyerName = model.CustomerName,
            BuyerNip = model.CustomerNip,
            BuyerAddress = model.CustomerAddress,
            GrossAmount = model.InvoiceTotal,
            NetAmount = model.SubTotal,
            VatAmount = model.VatAmount
        };
    }

    private static FileType GetFileTypeForModule(string moduleType)
    {
        return moduleType switch
        {
            nameof(ModuleType.Feeds) => FileType.FeedDeliveryInvoice,
            nameof(ModuleType.Gas) => FileType.GasDelivery,
            nameof(ModuleType.ProductionExpenses) => FileType.ExpenseProduction,
            nameof(ModuleType.Sales) => FileType.SalesInvoices,
            _ => FileType.AccountingInvoice
        };
    }
}

public class UploadAccountingInvoicesCommandValidator : AbstractValidator<UploadAccountingInvoicesCommand>
{
    public UploadAccountingInvoicesCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().WithMessage("Dane są wymagane.");
        RuleFor(t => t.Data.Files).NotNull().NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
        RuleFor(t => t.Data.InvoiceType).NotEmpty().WithMessage("Typ faktury jest wymagany.");
    }
}

public class UploadAccountingInvoicesCommandProfile : Profile
{
    public UploadAccountingInvoicesCommandProfile()
    {
        CreateMap<AccountingInvoiceModel, AccountingInvoiceExtractedData>()
            .ForMember(m => m.InvoiceDate,
                opt => opt.MapFrom(t => t.InvoiceDate.HasValue ? t.InvoiceDate.Value.ToString("yyyy-MM-dd") : null))
            .ForMember(m => m.DueDate,
                opt => opt.MapFrom(t => t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null))
            .ForMember(m => m.InvoiceType, opt => opt.Ignore())
            .ForMember(m => m.FarmId, opt => opt.Ignore())
            .ForMember(m => m.ModuleType, opt => opt.Ignore());
    }
}
