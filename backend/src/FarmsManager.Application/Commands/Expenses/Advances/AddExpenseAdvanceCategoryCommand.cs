using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Expenses.Advances;

public record AddExpenseAdvanceCategoryField(ExpenseAdvanceCategoryType Type, string Name);

public record AddExpenseAdvanceCategoryCommand(List<AddExpenseAdvanceCategoryField> Data)
    : IRequest<EmptyBaseResponse>;

public class
    AddExpenseAdvanceCategoryCommandHandler : IRequestHandler<AddExpenseAdvanceCategoryCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvanceCategoryRepository _expenseAdvanceCategoryRepository;

    public AddExpenseAdvanceCategoryCommandHandler(IUserDataResolver userDataResolver,
        IExpenseAdvanceCategoryRepository expenseAdvanceCategoryRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseAdvanceCategoryRepository = expenseAdvanceCategoryRepository;
    }


    public async Task<EmptyBaseResponse> Handle(AddExpenseAdvanceCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        List<ExpenseAdvanceCategoryEntity> entitiesToAdd = [];
        foreach (var addExpenseAdvanceCategoryField in request.Data)
        {
            var existedCategory =
                await _expenseAdvanceCategoryRepository.AnyAsync(
                    new GetExpenseAdvanceCategoryByTypeAndNameSpec(addExpenseAdvanceCategoryField.Type,
                        addExpenseAdvanceCategoryField.Name),
                    cancellationToken);
            if (existedCategory)
            {
                throw new Exception("Kategoria z podaną nazwą istnieje juz w bazie");
            }

            var newCategory = ExpenseAdvanceCategoryEntity.CreateNew(addExpenseAdvanceCategoryField.Name,
                addExpenseAdvanceCategoryField.Type, userId);

            entitiesToAdd.Add(newCategory);
        }

        if (entitiesToAdd.Count != 0)
        {
            await _expenseAdvanceCategoryRepository.AddRangeAsync(entitiesToAdd, cancellationToken);
        }

        return BaseResponse.EmptyResponse;
    }
}

public class AddExpenseAdvanceCategoryCommandValidator : AbstractValidator<AddExpenseAdvanceCategoryCommand>
{
    public AddExpenseAdvanceCategoryCommandValidator()
    {
        RuleFor(x => x.Data).NotEmpty();
        RuleForEach(t => t.Data).ChildRules(x =>
            x.RuleFor(t => t.Name).NotEmpty().MaximumLength(200)
                .WithMessage("Nazwa nie moze być dłuzsza nic 200 znaków"));
    }
}

public sealed class GetExpenseAdvanceCategoryByTypeAndNameSpec : BaseSpecification<ExpenseAdvanceCategoryEntity>
{
    public GetExpenseAdvanceCategoryByTypeAndNameSpec(ExpenseAdvanceCategoryType type, string name)
    {
        EnsureExists();
        Query.Where(t => t.Type == type);
        Query.Where(t => EF.Functions.ILike(t.Name, name));
    }
}