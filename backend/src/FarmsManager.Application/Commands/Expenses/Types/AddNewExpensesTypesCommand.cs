using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Types;

public record AddNewExpensesTypesCommand(string[] Types) : IRequest<EmptyBaseResponse>;

public class AddNewExpensesTypesCommandHandler : IRequestHandler<AddNewExpensesTypesCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseTypeRepository _expenseTypeRepository;

    public AddNewExpensesTypesCommandHandler(IUserDataResolver userDataResolver,
        IExpenseTypeRepository expenseTypeRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseTypeRepository = expenseTypeRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddNewExpensesTypesCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var items = request.Types.Select(type => ExpenseTypeEntity.CreateNew(type, userId)).ToList();

        await _expenseTypeRepository.AddRangeAsync(items, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AddNewExpensesTypesCommandValidator : AbstractValidator<AddNewExpensesTypesCommand>
{
    public AddNewExpensesTypesCommandValidator()
    {
        RuleFor(x => x.Types)
            .NotEmpty()
            .WithMessage("Lista typów nie może byc pusta");

        RuleForEach(x => x.Types)
            .NotEmpty()
            .WithMessage("Typ nie może być puste");
    }
}