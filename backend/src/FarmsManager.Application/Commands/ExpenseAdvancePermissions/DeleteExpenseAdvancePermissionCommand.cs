using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ExpenseAdvancePermissions;

public record DeleteExpenseAdvancePermissionCommand(Guid PermissionId) : IRequest<EmptyBaseResponse>;

public class DeleteExpenseAdvancePermissionCommandHandler : IRequestHandler<DeleteExpenseAdvancePermissionCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;

    public DeleteExpenseAdvancePermissionCommandHandler(
        IUserDataResolver userDataResolver,
        IExpenseAdvancePermissionRepository permissionRepository)
    {
        _userDataResolver = userDataResolver;
        _permissionRepository = permissionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(
        DeleteExpenseAdvancePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission == null)
        {
            throw DomainException.RecordNotFound("Uprawnienie nie istnieje.");
        }

        permission.Delete(currentUserId);

        return BaseResponse.EmptyResponse;
    }
}

public class DeleteExpenseAdvancePermissionCommandValidator : AbstractValidator<DeleteExpenseAdvancePermissionCommand>
{
    public DeleteExpenseAdvancePermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId)
            .NotEmpty()
            .WithMessage("PermissionId jest wymagane.");
    }
}
