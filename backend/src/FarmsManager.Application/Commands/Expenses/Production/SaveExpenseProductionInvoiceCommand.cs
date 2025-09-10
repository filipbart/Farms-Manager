using FarmsManager.Application.Commands.Expenses.Contractors;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
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

public class
    SaveExpenseProductionInvoiceCommandHandler : IRequestHandler<SaveExpenseProductionInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;

    public SaveExpenseProductionInvoiceCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IUserDataResolver userDataResolver,
        IExpenseContractorRepository expenseContractorRepository,
        IExpenseProductionRepository expenseProductionRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userDataResolver = userDataResolver;
        _expenseContractorRepository = expenseContractorRepository;
        _expenseProductionRepository = expenseProductionRepository;
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

        var existedInvoice = await _expenseProductionRepository.SingleOrDefaultAsync(
            new GetExpenseProductionInvoiceByInvoiceNumberSpec(request.Data.InvoiceNumber), ct);
        if (existedInvoice is not null)
        {
            throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
        }

        var newExpenseProduction = ExpenseProductionEntity.CreateNew(
            farm.Id,
            cycle.Id,
            contractor.Id,
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

        if (contractor.ExpenseTypeId is null)
        {
            contractor.SetExpenseType(request.Data.ExpenseTypeId!.Value);
            await _expenseContractorRepository.UpdateAsync(contractor, ct);
        }

        return response;
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
        RuleFor(x => x.Data.InvoiceTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.SubTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.VatAmount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.Comment).MaximumLength(500);
    }
}