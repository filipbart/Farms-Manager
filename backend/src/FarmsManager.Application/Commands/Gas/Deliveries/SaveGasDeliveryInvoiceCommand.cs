using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Deliveries;

public record SaveGasDeliveryInvoiceCommand : IRequest<EmptyBaseResponse>
{
    public string FilePath { get; init; }
    public Guid DraftId { get; init; }
    public AddGasDeliveryInvoiceDto Data { get; init; }
}

public class SaveGasDeliveryInvoiceCommandHandler : IRequestHandler<SaveGasDeliveryInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public SaveGasDeliveryInvoiceCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        IUserDataResolver userDataResolver,
        IGasContractorRepository gasContractorRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IExpenseProductionRepository expenseProductionRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _gasContractorRepository = gasContractorRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SaveGasDeliveryInvoiceCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId!.Value), ct);
        var contractor =
            await _gasContractorRepository.GetAsync(
                new GetGasContractorByIdSpec(request.Data.ContractorId!.Value), ct);

        if (await _s3Service.FileExistsAsync(FileType.GasDelivery, request.FilePath) == false)
        {
            response.AddError("FileUrl", "Nie znaleziono pliku");
            return response;
        }

        await CheckForDuplicatesAsync(request.Data.InvoiceNumber, ct);

        var newGasDelivery = GasDeliveryEntity.CreateNew(
            farm.Id,
            contractor.Id,
            request.Data.InvoiceNumber!,
            request.Data.InvoiceDate!.Value,
            request.Data.UnitPrice!.Value * request.Data.Quantity!.Value,
            request.Data.UnitPrice!.Value,
            request.Data.Quantity!.Value,
            request.Data.Comment,
            userId);

        var newPath = request.FilePath.Replace(request.DraftId.ToString(), newGasDelivery.Id.ToString())
            .Replace("draft", "saved");
        newGasDelivery.SetFilePath(newPath);

        await _gasDeliveryRepository.AddAsync(newGasDelivery, ct);

        await _s3Service.MoveFileAsync(FileType.GasDelivery, request.FilePath, newPath);

        return response;
    }

    private async Task CheckForDuplicatesAsync(string invoiceNumber, CancellationToken ct)
    {
        var existsInGas = await _gasDeliveryRepository.AnyAsync(
            new GetGasDeliveryByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInGas)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach gazu.");
        }
        
        var existsInFeeds = await _feedInvoiceRepository.AnyAsync(
            new GetFeedInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInFeeds)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach pasz.");
        }
        
        var existsInExpenses = await _expenseProductionRepository.AnyAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInExpenses)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w kosztach produkcyjnych.");
        }
        
        var existsInSales = await _saleInvoiceRepository.AnyAsync(
            new GetSaleInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);
        
        if (existsInSales)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w fakturach sprzedażowych.");
        }
    }
}

public class SaveGasDeliveryInvoiceCommandValidator : AbstractValidator<SaveGasDeliveryInvoiceCommand>
{
    public SaveGasDeliveryInvoiceCommandValidator()
    {
        RuleFor(x => x.FilePath).NotNull().NotEmpty();
        RuleFor(x => x.DraftId).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.FarmId).NotEmpty();
        RuleFor(x => x.Data.ContractorId).NotEmpty();
        RuleFor(x => x.Data.InvoiceNumber).NotEmpty();
        RuleFor(x => x.Data.InvoiceDate).NotEmpty();
        RuleFor(x => x.Data.UnitPrice).GreaterThan(0);
        RuleFor(x => x.Data.Quantity).GreaterThan(0);
    }
}