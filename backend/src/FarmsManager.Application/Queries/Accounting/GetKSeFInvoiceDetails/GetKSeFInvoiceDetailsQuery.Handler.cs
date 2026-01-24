using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;

public class GetKSeFInvoiceDetailsQueryHandler 
    : IRequestHandler<GetKSeFInvoiceDetailsQuery, BaseResponse<KSeFInvoiceDetailsDto>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public GetKSeFInvoiceDetailsQueryHandler(
        IKSeFInvoiceRepository invoiceRepository, 
        IMapper mapper,
        IS3Service s3Service,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IExpenseProductionRepository expenseProductionRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
        _s3Service = s3Service;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<BaseResponse<KSeFInvoiceDetailsDto>> Handle(
        GetKSeFInvoiceDetailsQuery request, 
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            new GetKSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        if (invoice == null)
        {
            var errorResponse = new BaseResponse<KSeFInvoiceDetailsDto>();
            errorResponse.AddError("NotFound", "Faktura nie została znaleziona");
            return errorResponse;
        }

        var dto = _mapper.Map<KSeFInvoiceDetailsDto>(invoice);

        dto.AssignedEntityInvoiceId = invoice.AssignedEntityInvoiceId;

        // Pobierz dane z encji modułowej jeśli istnieje
        if (invoice.AssignedEntityInvoiceId.HasValue)
        {
            string filePath = null;
            Guid? farmId = null;
            Guid? cycleId = null;
            string location = null;
            
            var entityId = invoice.AssignedEntityInvoiceId.Value;

            switch (invoice.ModuleType)
            {
                case ModuleType.Feeds:
                    var feedInvoice = await _feedInvoiceRepository.FirstOrDefaultAsync(
                        new GetFeedInvoiceByIdSpec(entityId), cancellationToken);
                    if (feedInvoice != null)
                    {
                        filePath = feedInvoice.FilePath;
                        farmId = feedInvoice.FarmId;
                        cycleId = feedInvoice.CycleId;
                        location = feedInvoice.Farm?.Name;
                    }
                    break;

                case ModuleType.Gas:
                    var gasDelivery = await _gasDeliveryRepository.FirstOrDefaultAsync(
                        new GetGasDeliveryByIdSpec(entityId), cancellationToken);
                    if (gasDelivery != null)
                    {
                        filePath = gasDelivery.FilePath;
                        farmId = gasDelivery.FarmId;
                        location = gasDelivery.Farm?.Name;
                        dto.GasQuantity = gasDelivery.Quantity;
                        dto.GasUnitPrice = gasDelivery.UnitPrice;
                        dto.GasInvoiceTotal = gasDelivery.InvoiceTotal;
                    }
                    break;

                case ModuleType.ProductionExpenses:
                    var expenseProduction = await _expenseProductionRepository.FirstOrDefaultAsync(
                        new GetExpenseProductionByIdSpec(entityId), cancellationToken);
                    if (expenseProduction != null)
                    {
                        filePath = expenseProduction.FilePath;
                        farmId = expenseProduction.FarmId;
                        cycleId = expenseProduction.CycleId;
                        location = expenseProduction.Farm?.Name;
                    }
                    break;

                case ModuleType.Sales:
                    var saleInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                        new SaleInvoiceByIdSpec(entityId), cancellationToken);
                    if (saleInvoice != null)
                    {
                        filePath = saleInvoice.FilePath;
                        farmId = saleInvoice.FarmId;
                        cycleId = saleInvoice.CycleId;
                        location = saleInvoice.Farm?.Name;
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                dto.FilePath = filePath;
                dto.HasPdf = true;
            }
            
            // Uzupełnij brakujące dane w DTO z modułu
            if (!dto.FarmId.HasValue && farmId.HasValue)
            {
                dto.FarmId = farmId;
                dto.Location = location;
            }
            
            if (!dto.CycleId.HasValue && cycleId.HasValue)
            {
                dto.CycleId = cycleId;
                // Możemy tu też pobrać szczegóły cyklu jeśli potrzebne (identifier, year)
                // Ale wymagałoby to wczytania cyklu w specyfikacjach modułowych
            }
        }

        // Jeśli ścieżka pliku nadal jest pusta, a faktura jest manualna, spróbuj znaleźć plik w S3
        if (string.IsNullOrEmpty(dto.FilePath) && invoice.InvoiceSource == KSeFInvoiceSource.Manual)
        {
            var possibleExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var moduleFileType = GetFileTypeForModule(invoice.ModuleType);
            
            foreach (var ext in possibleExtensions)
            {
                // First check module-specific folder using module FileType
                if (moduleFileType != FileType.AccountingInvoice)
                {
                    var modulePath = $"saved/{invoice.Id}{ext}";
                    try
                    {
                        if (await _s3Service.FileExistsAsync(moduleFileType, modulePath))
                        {
                            dto.FilePath = modulePath;
                            dto.HasPdf = true;
                            break;
                        }
                    }
                    catch {}
                }
                
                // Then check accounting folders (for backwards compatibility)
                var accountingPaths = new[] 
                { 
                    $"saved/{invoice.Id}{ext}",
                    $"{invoice.Id}{ext}"
                };

                foreach (var path in accountingPaths)
                {
                    try
                    {
                        if (await _s3Service.FileExistsAsync(FileType.AccountingInvoice, path))
                        {
                            dto.FilePath = path;
                            dto.HasPdf = true;
                            break;
                        }
                    }
                    catch {}
                }
                
                if (!string.IsNullOrEmpty(dto.FilePath))
                    break;
            }
        }

        return BaseResponse.CreateResponse(dto);
    }

    private static FileType GetFileTypeForModule(ModuleType? moduleType)
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
}
