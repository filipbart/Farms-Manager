using FarmsManager.Application.Commands.Expenses.Contractors;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
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
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record AddExpenseProductionData
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid ExpenseContractorId { get; init; }
    public Guid ExpenseTypeId { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public string Comment { get; init; }
    public IFormFile File { get; init; }
}

public record AddExpenseProductionCommand(AddExpenseProductionData Data) : IRequest<EmptyBaseResponse>;

public class AddExpenseProductionCommandHandler : IRequestHandler<AddExpenseProductionCommand, EmptyBaseResponse>
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

    public AddExpenseProductionCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
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

    public async Task<EmptyBaseResponse> Handle(AddExpenseProductionCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), ct);
        var expenseContractor =
            await _expenseContractorRepository.GetAsync(
                new GetExpenseContractorByIdSpec(request.Data.ExpenseContractorId), ct);

        // Sprawdź duplikaty we wszystkich modułach
        await CheckForDuplicatesAsync(request.Data.InvoiceNumber, ct);

        var newExpenseProduction = ExpenseProductionEntity.CreateNew(farm.Id, cycle.Id, expenseContractor.Id,
            request.Data.ExpenseTypeId, request.Data.InvoiceNumber, request.Data.InvoiceTotal, request.Data.SubTotal, 
            request.Data.VatAmount, request.Data.InvoiceDate, request.Data.Comment, userId);

        if (request.Data.File != null)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(request.Data.File.FileName);
            var filePath = "saved/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await request.Data.File.CopyToAsync(memoryStream, ct);
            var fileBytes = memoryStream.ToArray();

            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.ExpenseProduction, filePath, ct);
            newExpenseProduction.SetFilePath(key);
        }

        await _expenseProductionRepository.AddAsync(newExpenseProduction, ct);
        return new EmptyBaseResponse();
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

public class AddExpenseProductionCommandValidator : AbstractValidator<AddExpenseProductionCommand>
{
    public AddExpenseProductionCommandValidator()
    {
        RuleFor(t => t.Data.FarmId).NotEmpty();
        RuleFor(t => t.Data.CycleId).NotEmpty();
        RuleFor(t => t.Data.ExpenseContractorId).NotEmpty();
        RuleFor(t => t.Data.ExpenseTypeId).NotEmpty();
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.Comment).MaximumLength(500);
    }
}