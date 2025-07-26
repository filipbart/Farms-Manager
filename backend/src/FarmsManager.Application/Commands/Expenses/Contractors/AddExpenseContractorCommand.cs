using Ardalis.Specification;
using FarmsManager.Application.Commands.Expenses.Types;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Expenses.Contractors;

public record AddExpenseContractorDto
{
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public Guid ExpenseTypeId { get; init; }
}

public record AddExpenseContractorCommand(AddExpenseContractorDto Data) : IRequest<EmptyBaseResponse>;

public class AddExpenseContractorCommandHandler : IRequestHandler<AddExpenseContractorCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseTypeRepository _expenseTypeRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;

    public AddExpenseContractorCommandHandler(IUserDataResolver userDataResolver,
        IExpenseTypeRepository expenseTypeRepository, IExpenseContractorRepository expenseContractorRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseTypeRepository = expenseTypeRepository;
        _expenseContractorRepository = expenseContractorRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddExpenseContractorCommand request,
        CancellationToken cancellationToken)
    {
        var response = BaseResponse.EmptyResponse;
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var expenseType = await _expenseTypeRepository.GetAsync(new GetExpenseTypeByIdSpec(request.Data.ExpenseTypeId),
            cancellationToken);

        var existedExpenseContractor = await _expenseContractorRepository.FirstOrDefaultAsync(
            new GetExpenseContractorByNipOrNameSpec(request.Data.Nip, request.Data.Name), cancellationToken);
        if (existedExpenseContractor is not null)
        {
            response.AddError("ExistedNameOrNip", "Istnieje już kontrahent z podanym numerem NIP lub nazwą");
            return response;
        }

        var newContractor = ExpenseContractorEntity.CreateNew(expenseType.Id, request.Data.Name,
            request.Data.Nip, request.Data.Address, userId);

        await _expenseContractorRepository.AddAsync(newContractor, cancellationToken);
        return response;
    }
}

public class AddExpenseContractorCommandValidator : AbstractValidator<AddExpenseContractorCommand>
{
    public AddExpenseContractorCommandValidator()
    {
        RuleFor(t => t.Data.Name).NotEmpty();
        RuleFor(t => t.Data.Nip).NotEmpty();
        RuleFor(t => t.Data.Address).NotEmpty();
        RuleFor(t => t.Data.ExpenseTypeId).NotEmpty();
    }
}

public sealed class GetExpenseContractorByNipOrNameSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public GetExpenseContractorByNipOrNameSpec(string nip, string name)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(t.Nip, nip) || EF.Functions.ILike(t.Name, name));
    }
}