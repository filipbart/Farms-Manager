using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Application.Specifications.TaxBusinessEntities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using KSeF.Client.Core.Models.Invoices.Common;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record SaveAccountingInvoiceDto
{
    public Guid DraftId { get; init; }
    public string FilePath { get; init; }
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public string SellerName { get; init; }
    public string SellerNip { get; init; }
    public string BuyerName { get; init; }
    public string BuyerNip { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal NetAmount { get; init; }
    public decimal VatAmount { get; init; }
    public string InvoiceType { get; init; }
    public string DocumentType { get; init; } // Vat, Kor, Zal, Roz, etc.
    public string Status { get; init; } // New, Accepted, etc.
    public string VatDeductionType { get; init; } // Full, Half, None
    public ModuleType ModuleType { get; init; }
    public string Comment { get; init; }
    public string PaymentStatus { get; init; }
    public DateOnly? PaymentDate { get; init; }
    
    // Module-specific data
    public SaveFeedInvoiceDto FeedData { get; init; }
    public SaveGasDeliveryDto GasData { get; init; }
    public SaveExpenseProductionDto ExpenseData { get; init; }
    public SaveSaleInvoiceDto SaleData { get; init; }
}

#region Module DTOs

public record SaveFeedInvoiceDto
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }
    public string BankAccountNumber { get; init; }
    public string VendorName { get; init; }
    public string ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public record SaveGasDeliveryDto
{
    public Guid FarmId { get; init; }
    public Guid? ContractorId { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
}

public record SaveExpenseProductionDto
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid? ExpenseContractorId { get; init; }
    public Guid ExpenseTypeId { get; init; }
}

public record SaveSaleInvoiceDto
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid? SlaughterhouseId { get; init; }
}

#endregion

public record SaveAccountingInvoiceCommand(SaveAccountingInvoiceDto Data)
    : IRequest<BaseResponse<Guid>>;

public class SaveAccountingInvoiceCommandHandler : IRequestHandler<SaveAccountingInvoiceCommand, BaseResponse<Guid>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IS3Service _s3Service;
    private readonly ITaxBusinessEntityRepository _taxBusinessEntityRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;

    public SaveAccountingInvoiceCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IUserDataResolver userDataResolver,
        IS3Service s3Service,
        ITaxBusinessEntityRepository taxBusinessEntityRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IGasContractorRepository gasContractorRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IExpenseContractorRepository expenseContractorRepository,
        ISaleInvoiceRepository saleInvoiceRepository,
        ISlaughterhouseRepository slaughterhouseRepository)
    {
        _invoiceRepository = invoiceRepository;
        _userDataResolver = userDataResolver;
        _s3Service = s3Service;
        _taxBusinessEntityRepository = taxBusinessEntityRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _gasContractorRepository = gasContractorRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _expenseContractorRepository = expenseContractorRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _slaughterhouseRepository = slaughterhouseRepository;
    }

    public async Task<BaseResponse<Guid>> Handle(SaveAccountingInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var data = request.Data;

        var invoiceDate = data.InvoiceDate;
        var paymentDueDate = data.DueDate;

        // Override Seller/Buyer details from module entities if available
        var sellerName = data.SellerName;
        var sellerNip = data.SellerNip;
        var buyerName = data.BuyerName;
        var buyerNip = data.BuyerNip;

        if (data.ModuleType == ModuleType.ProductionExpenses && data.ExpenseData?.ExpenseContractorId.HasValue == true)
        {
            var contractor = await _expenseContractorRepository.GetByIdAsync(data.ExpenseData.ExpenseContractorId.Value, cancellationToken);
            if (contractor != null)
            {
                sellerName = contractor.Name;
                sellerNip = contractor.Nip;
            }
        }
        else if (data.ModuleType == ModuleType.Gas && data.GasData?.ContractorId.HasValue == true)
        {
            var contractor = await _gasContractorRepository.GetByIdAsync(data.GasData.ContractorId.Value, cancellationToken);
            if (contractor != null)
            {
                sellerName = contractor.Name;
                sellerNip = contractor.Nip;
            }
        }
        else if (data.ModuleType == ModuleType.Sales && data.SaleData?.SlaughterhouseId.HasValue == true)
        {
            var slaughterhouse = await _slaughterhouseRepository.GetByIdAsync(data.SaleData.SlaughterhouseId.Value, cancellationToken);
            if (slaughterhouse != null)
            {
                buyerName = slaughterhouse.Name;
                buyerNip = slaughterhouse.Nip;
            }
        }

        // Określ kierunek faktury
        var invoiceDirection = data.InvoiceType == "Sales"
            ? KSeFInvoiceDirection.Sales
            : KSeFInvoiceDirection.Purchase;

        // Dopasuj podmiot gospodarczy po NIP sprzedawcy lub nabywcy
        var taxBusinessEntityId = await MatchTaxBusinessEntityAsync(
            sellerNip, sellerName, buyerNip, buyerName, cancellationToken);

        // Sprawdź duplikaty przed zapisem
        await CheckForDuplicatesAsync(data.InvoiceNumber, sellerNip, taxBusinessEntityId, cancellationToken);

        // Dla manualnych faktur generujemy placeholder KSeFNumber
        var manualKSeFNumber = $"MANUAL-{data.DraftId:N}";

        // Określ status faktury
        var invoiceStatus = ParseInvoiceStatus(data.Status);
        
        // Encję modułową tworzymy zawsze dla faktur manualnych (a ten command obsługuje tylko manualne)
        var shouldCreateModuleEntity = true;

        // Pobierz dane lokalizacji i cyklu z danych modułu
        Guid? farmId = null;
        Guid? cycleId = null;

        switch (data.ModuleType)
        {
            case ModuleType.Feeds:
                farmId = data.FeedData?.FarmId;
                cycleId = data.FeedData?.CycleId;
                break;
            case ModuleType.Gas:
                farmId = data.GasData?.FarmId;
                break;
            case ModuleType.ProductionExpenses:
                farmId = data.ExpenseData?.FarmId;
                cycleId = data.ExpenseData?.CycleId;
                break;
            case ModuleType.Sales:
                farmId = data.SaleData?.FarmId;
                cycleId = data.SaleData?.CycleId;
                break;
        }

        // Utwórz encję faktury
        var invoice = KSeFInvoiceEntity.CreateNew(
            kSeFNumber: manualKSeFNumber,
            invoiceNumber: data.InvoiceNumber,
            invoiceDate: invoiceDate,
            paymentDueDate: paymentDueDate,
            sellerNip: sellerNip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            sellerName: sellerName,
            buyerNip: buyerNip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            buyerName: buyerName,
            invoiceType: ParseDocumentType(data.DocumentType),
            status: invoiceStatus,
            paymentStatus: ParsePaymentStatus(data.PaymentStatus),
            paymentType: KSeFInvoicePaymentType.BankTransfer,
            vatDeductionType: ParseVatDeductionType(data.VatDeductionType),
            moduleType: data.ModuleType,
            invoiceXml: string.Empty, // Brak XML dla manualnych faktur
            invoiceDirection: invoiceDirection,
            invoiceSource: KSeFInvoiceSource.Manual,
            grossAmount: data.GrossAmount,
            netAmount: data.NetAmount,
            vatAmount: data.VatAmount,
            comment: data.Comment,
            userId: userId,
            assignedUserId: userId,
            taxBusinessEntityId: taxBusinessEntityId,
            farmId: farmId,
            cycleId: cycleId,
            paymentDate: data.PaymentDate
        );
        
        await _invoiceRepository.AddAsync(invoice, cancellationToken);

        // Construct permanent path in saved folder using module-specific FileType
        var extension = Path.GetExtension(data.FilePath);
        var fileType = GetFileTypeForModule(data.ModuleType);
        var relativePath = $"saved/{invoice.Id}{extension}";
        
        // Move file from draft to saved
        await _s3Service.MoveFileAsync(fileType, data.FilePath, relativePath);
        
        // Store full path with module prefix for frontend retrieval
        var modulePrefix = GetModulePrefixForFileType(fileType);
        var fullPath = $"{modulePrefix}/{relativePath}";

        // Utwórz encję modułową jeśli wymagane
        if (shouldCreateModuleEntity)
        {
            var moduleEntityId = await CreateModuleEntityAsync(invoice, data, userId, fullPath, cancellationToken);
            if (moduleEntityId.HasValue)
            {
                invoice.SetAssignedEntityInvoiceId(moduleEntityId.Value);
            }
        }

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(invoice.Id);
    }

    /// <summary>
    /// Dopasowuje podmiot gospodarczy na podstawie NIP lub nazwy
    /// </summary>
    private async Task<Guid?> MatchTaxBusinessEntityAsync(
        string sellerNip, string sellerName, string buyerNip, string buyerName,
        CancellationToken cancellationToken)
    {
        var taxBusinessEntities = await _taxBusinessEntityRepository
            .ListAsync(new AllActiveTaxBusinessEntitiesSpec(), cancellationToken);

        if (taxBusinessEntities.Count == 0)
            return null;

        // Normalizuj NIP-y
        sellerNip = NormalizeNip(sellerNip);
        buyerNip = NormalizeNip(buyerNip);

        // Szukaj po NIP (dokładne dopasowanie)
        var matchedByNip = taxBusinessEntities.FirstOrDefault(t =>
            t.Nip == sellerNip || t.Nip == buyerNip);

        if (matchedByNip != null)
            return matchedByNip.Id;

        // Szukaj po nazwie (częściowe dopasowanie)
        var sellerNameLower = sellerName?.ToLowerInvariant();
        var buyerNameLower = buyerName?.ToLowerInvariant();

        var matchedByName = taxBusinessEntities.FirstOrDefault(t =>
        {
            var entityName = t.Name?.ToLowerInvariant();
            if (string.IsNullOrEmpty(entityName))
                return false;

            return (!string.IsNullOrEmpty(sellerNameLower) && sellerNameLower.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(buyerNameLower) && buyerNameLower.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(sellerNameLower) && entityName.Contains(sellerNameLower)) ||
                   (!string.IsNullOrEmpty(buyerNameLower) && entityName.Contains(buyerNameLower));
        });

        return matchedByName?.Id;
    }

    private async Task CheckForDuplicatesAsync(string invoiceNumber, string sellerNip, Guid? taxBusinessEntityId, CancellationToken cancellationToken)
    {
        var normalizedSellerNip = NormalizeNip(sellerNip);
        
        // Sprawdź duplikaty w fakturach KSeF
        var existsInKSeF = await _invoiceRepository.AnyAsync(
            new KSeFInvoiceByNumberAndSellerSpec(invoiceNumber, normalizedSellerNip, taxBusinessEntityId),
            cancellationToken);

        if (existsInKSeF)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' od tego sprzedawcy już istnieje w księgowości.");
        }
        
        // Sprawdź duplikaty w dostawach gazu
        var existsInGas = await _gasDeliveryRepository.AnyAsync(
            new GetGasDeliveryByInvoiceNumberSpec(invoiceNumber),
            cancellationToken);
        
        if (existsInGas)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach gazu.");
        }
        
        // Sprawdź duplikaty w dostawach pasz
        var existsInFeeds = await _feedInvoiceRepository.AnyAsync(
            new GetFeedInvoiceByInvoiceNumberSpec(invoiceNumber),
            cancellationToken);
        
        if (existsInFeeds)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach pasz.");
        }
        
        // Sprawdź duplikaty w kosztach produkcyjnych
        var existsInExpenses = await _expenseProductionRepository.AnyAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(invoiceNumber),
            cancellationToken);
        
        if (existsInExpenses)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w kosztach produkcyjnych.");
        }
        
        // Sprawdź duplikaty w fakturach sprzedażowych
        var existsInSales = await _saleInvoiceRepository.AnyAsync(
            new GetSaleInvoiceByInvoiceNumberSpec(invoiceNumber),
            cancellationToken);
        
        if (existsInSales)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w fakturach sprzedażowych.");
        }
    }

    /// <summary>
    /// Tworzy encję modułową na podstawie danych faktury
    /// </summary>
    private async Task<Guid?> CreateModuleEntityAsync(KSeFInvoiceEntity invoice, SaveAccountingInvoiceDto data, Guid userId, string filePath, CancellationToken cancellationToken)
    {
        switch (data.ModuleType)
        {
            case ModuleType.Feeds:
                if (data.FeedData != null)
                {
                    var feedInvoice = FeedInvoiceEntity.CreateNew(
                        data.FeedData.FarmId,
                        data.FeedData.CycleId,
                        data.FeedData.HenhouseId,
                        data.InvoiceNumber,
                        data.FeedData.BankAccountNumber ?? "",
                        data.FeedData.VendorName,
                        data.FeedData.ItemName,
                        data.FeedData.Quantity,
                        data.FeedData.UnitPrice,
                        data.InvoiceDate,
                        data.DueDate ?? data.InvoiceDate,
                        data.GrossAmount,
                        data.NetAmount,
                        data.VatAmount,
                        data.Comment,
                        userId);
                    
                    feedInvoice.SetFilePath(filePath);

                    await _feedInvoiceRepository.AddAsync(feedInvoice, cancellationToken);
                    await _feedInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                    return feedInvoice.Id;
                }
                break;

            case ModuleType.Gas:
                if (data.GasData != null)
                {
                    var gasDelivery = GasDeliveryEntity.CreateNew(
                        data.GasData.FarmId,
                        data.GasData.ContractorId ?? Guid.Empty,
                        data.InvoiceNumber,
                        data.InvoiceDate,
                        data.GrossAmount,
                        data.GasData.UnitPrice,
                        data.GasData.Quantity,
                        data.Comment,
                        userId);
                    
                    gasDelivery.SetFilePath(filePath);

                    await _gasDeliveryRepository.AddAsync(gasDelivery, cancellationToken);
                    await _gasDeliveryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                    return gasDelivery.Id;
                }
                break;

            case ModuleType.ProductionExpenses:
                if (data.ExpenseData != null)
                {
                    var expenseProduction = ExpenseProductionEntity.CreateNew(
                        data.ExpenseData.FarmId,
                        data.ExpenseData.CycleId,
                        data.ExpenseData.ExpenseContractorId ?? Guid.Empty,
                        data.ExpenseData.ExpenseTypeId,
                        data.InvoiceNumber,
                        data.GrossAmount,
                        data.NetAmount,
                        data.VatAmount,
                        data.InvoiceDate,
                        data.Comment,
                        userId);
                    
                    expenseProduction.SetFilePath(filePath);

                    await _expenseProductionRepository.AddAsync(expenseProduction, cancellationToken);
                    
                    // Dodaj typ wydatku do kontrahenta, jeśli jeszcze go nie ma
                    if (data.ExpenseData.ExpenseContractorId.HasValue && data.ExpenseData.ExpenseTypeId != Guid.Empty)
                    {
                        var contractor = await _expenseContractorRepository.GetByIdAsync(
                            data.ExpenseData.ExpenseContractorId.Value, cancellationToken);
                        
                        if (contractor != null)
                        {
                            contractor.AddExpenseType(data.ExpenseData.ExpenseTypeId, userId);
                        }
                    }
                    
                    await _expenseProductionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                    return expenseProduction.Id;
                }
                break;

            case ModuleType.Sales:
                if (data.SaleData != null)
                {
                    var saleInvoice = SaleInvoiceEntity.CreateNew(
                        data.SaleData.FarmId,
                        data.SaleData.CycleId,
                        data.SaleData.SlaughterhouseId ?? Guid.Empty,
                        data.InvoiceNumber,
                        data.InvoiceDate,
                        data.DueDate ?? data.InvoiceDate,
                        data.GrossAmount,
                        data.NetAmount,
                        data.VatAmount,
                        userId);
                    
                    saleInvoice.SetFilePath(filePath);

                    await _saleInvoiceRepository.AddAsync(saleInvoice, cancellationToken);
                    await _saleInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                    return saleInvoice.Id;
                }
                break;

            case ModuleType.Farmstead:
            case ModuleType.Other:
            case ModuleType.None:
                // Dla tych modułów nie tworzymy encji modułowej
                break;
        }

        return null;
    }

    private static string NormalizeNip(string nip)
    {
        return nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    }

    private static KSeFPaymentStatus ParsePaymentStatus(string paymentStatus)
    {
        return paymentStatus switch
        {
            "Unpaid" => KSeFPaymentStatus.Unpaid,
            "PartiallyPaid" => KSeFPaymentStatus.PartiallyPaid,
            "Suspended" => KSeFPaymentStatus.Suspended,
            "PaidCash" => KSeFPaymentStatus.PaidCash,
            "PaidTransfer" => KSeFPaymentStatus.PaidTransfer,
            _ => KSeFPaymentStatus.Unpaid
        };
    }

    private static InvoiceType ParseDocumentType(string documentType)
    {
        return documentType switch
        {
            "Vat" => InvoiceType.Vat,
            "Zal" => InvoiceType.Zal,
            "Kor" => InvoiceType.Kor,
            "Roz" => InvoiceType.Roz,
            "Upr" => InvoiceType.Upr,
            "KorZal" => InvoiceType.KorZal,
            "KorRoz" => InvoiceType.KorRoz,
            "VatPef" => InvoiceType.VatPef,
            "VatPefSp" => InvoiceType.VatPefSp,
            "KorPef" => InvoiceType.KorPef,
            "VatRr" => InvoiceType.VatRr,
            "KorVatRr" => InvoiceType.KorVatRr,
            _ => InvoiceType.Vat
        };
    }

    private static KSeFInvoiceStatus ParseInvoiceStatus(string status)
    {
        return status switch
        {
            "New" => KSeFInvoiceStatus.New,
            "Rejected" => KSeFInvoiceStatus.Rejected,
            "Accepted" => KSeFInvoiceStatus.Accepted,
            "RequiresLinking" => KSeFInvoiceStatus.RequiresLinking,
            _ => KSeFInvoiceStatus.Accepted
        };
    }

    private static KSeFVatDeductionType ParseVatDeductionType(string vatDeductionType)
    {
        return vatDeductionType switch
        {
            "Full" => KSeFVatDeductionType.Full,
            "Half" => KSeFVatDeductionType.Half,
            "None" => KSeFVatDeductionType.None,
            _ => KSeFVatDeductionType.Full
        };
    }

    private static FileType GetFileTypeForModule(ModuleType moduleType)
    {
        return moduleType switch
        {
            ModuleType.Feeds => FileType.FeedDeliveryInvoice,
            ModuleType.Gas => FileType.GasDelivery,
            ModuleType.ProductionExpenses => FileType.ExpenseProduction,
            ModuleType.Sales => FileType.SalesInvoices,
            _ => FileType.AccountingInvoice
        };
    }

    private static string GetModulePrefixForFileType(FileType fileType)
    {
        return fileType switch
        {
            FileType.FeedDeliveryInvoice => "FeedDeliveryInvoice",
            FileType.GasDelivery => "GasDelivery",
            FileType.ExpenseProduction => "ExpenseProduction",
            FileType.SalesInvoices => "SalesInvoices",
            FileType.AccountingInvoice => "accounting",
            _ => fileType.ToString()
        };
    }
}

public class SaveAccountingInvoiceCommandValidator : AbstractValidator<SaveAccountingInvoiceCommand>
{
    public SaveAccountingInvoiceCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().WithMessage("Dane są wymagane.");
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty().WithMessage("Numer faktury jest wymagany.");
        RuleFor(t => t.Data.InvoiceDate).NotEmpty().WithMessage("Data faktury jest wymagana.");
        RuleFor(t => t.Data.InvoiceType).NotEmpty().WithMessage("Typ faktury jest wymagany.");
        RuleFor(t => t.Data.GrossAmount).GreaterThan(0).WithMessage("Kwota brutto musi być większa od 0.");
    }
}
