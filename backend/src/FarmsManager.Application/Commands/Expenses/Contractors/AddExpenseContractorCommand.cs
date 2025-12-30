using Ardalis.Specification;
using FarmsManager.Application.Commands.Expenses.Types;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
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
    public List<Guid> ExpenseTypeIds { get; init; } = new();
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

        var existedExpenseContractor = await _expenseContractorRepository.FirstOrDefaultAsync(
            new GetExpenseContractorByNipOrNameSpec(request.Data.Nip, request.Data.Name), cancellationToken);
        if (existedExpenseContractor is not null)
        {
            response.AddError("ExistedNameOrNip", "Istnieje już kontrahent z podanym numerem NIP lub nazwą");
            return response;
        }

        var newContractor = ExpenseContractorEntity.CreateNew(request.Data.Name,
            request.Data.Nip, request.Data.Address, request.Data.ExpenseTypeIds, userId);

        await _expenseContractorRepository.AddAsync(newContractor, cancellationToken);
        return response;
    }
}

public class AddExpenseContractorCommandValidator : AbstractValidator<AddExpenseContractorCommand>
{
    public AddExpenseContractorCommandValidator()
    {
        RuleFor(t => t.Data.Name).NotEmpty();
        RuleFor(t => t.Data.Nip).NotEmpty().Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
        RuleFor(t => t.Data.Address).NotEmpty();
        RuleFor(t => t.Data.ExpenseTypeIds).NotEmpty().WithMessage("Wymagany jest co najmniej jeden typ wydatku.");
    }
}

public sealed class GetExpenseContractorByNipOrNameSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public GetExpenseContractorByNipOrNameSpec(string nip, string name)
    {
        nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip || EF.Functions.ILike(t.Name, name));
    }
}