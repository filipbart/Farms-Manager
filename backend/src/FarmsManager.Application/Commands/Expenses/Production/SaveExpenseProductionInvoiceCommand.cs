using FarmsManager.Application.Commands.Expenses.Contractors;
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
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record SaveExpenseProductionInvoiceCommand : IRequest<EmptyBaseResponse>
{
    public string FilePath { get; init; }
    public Guid DraftId { get; init; }
    public AddExpenseProductionInvoiceDto Data { get; init; }
}

public class SaveExpenseProductionInvoiceCommandHandler : IRequestHandler<SaveExpenseProductionInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public SaveExpenseProductionInvoiceCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IUserDataResolver userDataResolver,
        IExpenseContractorRepository expenseContractorRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userDataResolver = userDataResolver;
        _expenseContractorRepository = expenseContractorRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SaveExpenseProductionInvoiceCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId!.Value), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId!.Value), ct);
        var contractor =
            await _expenseContractorRepository.GetAsync(
                new GetExpenseContractorByIdSpec(request.Data.ContractorId!.Value), ct);

        if (await _s3Service.FileExistsAsync(FileType.ExpenseProduction, request.FilePath) == false)
        {
            response.AddError("FileUrl", "Nie znaleziono pliku");
            return response;
        }

        // Sprawdź duplikaty we wszystkich modułach
        await CheckForDuplicatesAsync(request.Data.InvoiceNumber, ct);

        var newExpenseProduction = ExpenseProductionEntity.CreateNew(
            farm.Id,
            cycle.Id,
            contractor.Id,
            request.Data.ExpenseTypeId!.Value,
            request.Data.InvoiceNumber!,
            request.Data.InvoiceTotal!.Value,
            request.Data.SubTotal!.Value,
            request.Data.VatAmount!.Value,
            request.Data.InvoiceDate!.Value,
            request.Data.Comment,
            userId);

        var newPath = request.FilePath.Replace(request.DraftId.ToString(), newExpenseProduction.Id.ToString())
            .Replace("draft", "saved");
        newExpenseProduction.SetFilePath(newPath);

        await _expenseProductionRepository.AddAsync(newExpenseProduction, ct);

        await _s3Service.MoveFileAsync(FileType.ExpenseProduction, request.FilePath, newPath);

        return response;
    }

    private async Task CheckForDuplicatesAsync(string invoiceNumber, CancellationToken ct)
    {
        // Sprawdź duplikaty w kosztach produkcyjnych
        var existsInExpenses = await _expenseProductionRepository.AnyAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);

        if (existsInExpenses)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w kosztach produkcyjnych.");
        }

        // Sprawdź duplikaty w dostawach gazu
        var existsInGas = await _gasDeliveryRepository.AnyAsync(
            new GetGasDeliveryByInvoiceNumberSpec(invoiceNumber),
            ct);

        if (existsInGas)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach gazu.");
        }

        // Sprawdź duplikaty w dostawach pasz
        var existsInFeeds = await _feedInvoiceRepository.AnyAsync(
            new GetFeedInvoiceByInvoiceNumberSpec(invoiceNumber),
            ct);

        if (existsInFeeds)
        {
            throw DomainException.BadRequest($"Faktura o numerze '{invoiceNumber}' już istnieje w dostawach pasz.");
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

public class SaveExpenseProductionInvoiceCommandValidator : AbstractValidator<SaveExpenseProductionInvoiceCommand>
{
    public SaveExpenseProductionInvoiceCommandValidator()
    {
        RuleFor(x => x.FilePath).NotNull().NotEmpty();
        RuleFor(x => x.DraftId).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.FarmId).NotEmpty();
        RuleFor(x => x.Data.CycleId).NotEmpty();
        RuleFor(x => x.Data.ContractorId).NotEmpty();
        RuleFor(x => x.Data.ExpenseTypeId).NotEmpty();
        RuleFor(x => x.Data.InvoiceNumber).NotEmpty();
        RuleFor(x => x.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.Comment).MaximumLength(500);
    }
}