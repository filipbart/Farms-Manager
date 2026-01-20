using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record SaveFeedInvoiceDataCommand : IRequest<EmptyBaseResponse>
{
    public string FilePath { get; init; }
    public Guid DraftId { get; init; }
    public AddFeedDeliveryInvoiceDto Data { get; init; }
}

public class SaveFeedInvoiceDataCommandHandler : IRequestHandler<SaveFeedInvoiceDataCommand, EmptyBaseResponse>
{
    public SaveFeedInvoiceDataCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IHenhouseRepository henhouseRepository,
        IFeedNameRepository feedNameRepository, IFeedInvoiceRepository feedInvoiceRepository, IS3Service s3Service,
        IFeedPriceRepository feedPriceRepository, IGasDeliveryRepository gasDeliveryRepository,
        IExpenseProductionRepository expenseProductionRepository, ISaleInvoiceRepository saleInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _feedNameRepository = feedNameRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _s3Service = s3Service;
        _feedPriceRepository = feedPriceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IS3Service _s3Service;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public async Task<EmptyBaseResponse> Handle(SaveFeedInvoiceDataCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId!.Value), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId!.Value), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.Data.HenhouseId!.Value), ct);

        var feedName =
            await _feedNameRepository.FirstOrDefaultAsync(new GetFeedNameByNameSpec(request.Data.ItemName!), ct);
        if (feedName is null)
        {
            response.AddError("FeedName", "Nie znaleziono nazwy paszy w słowniku");
            return response;
        }

        if (await _s3Service.FileExistsAsync(FileType.FeedDeliveryInvoice, request.FilePath) == false)
        {
            response.AddError("FileUrl", "Nie znaleziono pliku");
            return response;
        }

        await CheckForDuplicatesAsync(request.Data.InvoiceNumber, ct);

        var newFeedInvoice = FeedInvoiceEntity.CreateNew(
            farm.Id,
            cycle.Id,
            henhouse.Id,
            request.Data.InvoiceNumber!,
            request.Data.BankAccountNumber!,
            request.Data.VendorName!,
            request.Data.ItemName!,
            request.Data.Quantity!.Value,
            request.Data.UnitPrice!.Value,
            request.Data.InvoiceDate!.Value,
            request.Data.DueDate!.Value,
            request.Data.InvoiceTotal!.Value,
            request.Data.SubTotal!.Value,
            request.Data.VatAmount!.Value,
            request.Data.Comment,
            userId);

        await CheckFeedInvoiceUnitPrice(newFeedInvoice, ct);

        var newPath = request.FilePath.Replace(request.DraftId.ToString(), newFeedInvoice.Id.ToString())
            .Replace("draft", "saved");
        newFeedInvoice.SetFilePath(newPath);

        await _feedInvoiceRepository.AddAsync(newFeedInvoice, ct);

        await _s3Service.MoveFileAsync(FileType.FeedDeliveryInvoice, request.FilePath, newPath);

        return response;
    }

    private async Task CheckFeedInvoiceUnitPrice(FeedInvoiceEntity feedInvoice, CancellationToken ct)
    {
        var feedPrices =
            await _feedPriceRepository.GetFeedPricesForInvoiceDateAsync(feedInvoice.FarmId, feedInvoice.CycleId,
                feedInvoice.ItemName,
                feedInvoice.InvoiceDate);

        feedInvoice.CheckUnitPrice(feedPrices);
    }

    private async Task CheckForDuplicatesAsync(string invoiceNumber, CancellationToken ct)
    {
        // Sprawdź duplikaty w dostawach pasz
        var existsInFeeds = await _feedInvoiceRepository.AnyAsync(
            new GetFeedInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInFeeds)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach pasz.");
        }
        
        // Sprawdź duplikaty w dostawach gazu
        var existsInGas = await _gasDeliveryRepository.AnyAsync(
            new GetGasDeliveryByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInGas)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach gazu.");
        }
        
        // Sprawdź duplikaty w kosztach produkcyjnych
        var existsInExpenses = await _expenseProductionRepository.AnyAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInExpenses)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w kosztach produkcyjnych.");
        }
        
        // Sprawdź duplikaty w fakturach sprzedażowych
        var existsInSales = await _saleInvoiceRepository.AnyAsync(
            new GetSaleInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInSales)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w fakturach sprzedażowych.");
        }
    }
}

public class SaveFeedInvoiceDataCommandValidator : AbstractValidator<SaveFeedInvoiceDataCommand>
{
    public SaveFeedInvoiceDataCommandValidator()
    {
        RuleFor(t => t.FilePath).NotEmpty();
        RuleFor(t => t.DraftId).NotEmpty();
        RuleFor(t => t.Data).NotNull();

        RuleFor(t => t.Data.FarmId).NotEmpty();
        RuleFor(t => t.Data.CycleId).NotEmpty();
        RuleFor(t => t.Data.HenhouseId).NotEmpty();

        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.BankAccountNumber).NotEmpty();
        RuleFor(t => t.Data.VendorName).NotEmpty();
        RuleFor(t => t.Data.ItemName).NotEmpty();
        RuleFor(t => t.Data.Quantity).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.UnitPrice).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.DueDate).NotEmpty();
        RuleFor(t => t.Data.InvoiceTotal).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.SubTotal).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.VatAmount).NotEmpty().GreaterThanOrEqualTo(0);
    }
}