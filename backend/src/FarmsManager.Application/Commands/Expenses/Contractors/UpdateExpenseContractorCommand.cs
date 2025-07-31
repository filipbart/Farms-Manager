using FarmsManager.Application.Commands.Expenses.Types;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Contractors;

public record UpdateExpenseContractorCommand(Guid ExpenseContractorId, AddExpenseContractorDto Data)
    : IRequest<EmptyBaseResponse>;

public class UpdateExpenseContractorCommandHandler : IRequestHandler<UpdateExpenseContractorCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseTypeRepository _expenseTypeRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;

    public UpdateExpenseContractorCommandHandler(IUserDataResolver userDataResolver,
        IExpenseTypeRepository expenseTypeRepository, IExpenseContractorRepository expenseContractorRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseTypeRepository = expenseTypeRepository;
        _expenseContractorRepository = expenseContractorRepository;
    }


    public async Task<EmptyBaseResponse> Handle(UpdateExpenseContractorCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseContractorRepository.GetAsync(new GetExpenseContractorByIdSpec(request.ExpenseContractorId),
                cancellationToken);

        var expenseType = await _expenseTypeRepository.GetAsync(new GetExpenseTypeByIdSpec(request.Data.ExpenseTypeId),
            cancellationToken);


        entity.Update(expenseType.Id, request.Data.Name, request.Data.Nip, request.Data.Address);
        entity.SetModified(userId);
        await _expenseContractorRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateExpenseContractorCommandValidator : AbstractValidator<UpdateExpenseContractorCommand>
{
    public UpdateExpenseContractorCommandValidator()
    {
        RuleFor(t => t.Data.Name).NotEmpty();
        RuleFor(t => t.Data.Nip).NotEmpty().Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
        RuleFor(t => t.Data.Address).NotEmpty();
        RuleFor(t => t.Data.ExpenseTypeId).NotEmpty();
    }
}