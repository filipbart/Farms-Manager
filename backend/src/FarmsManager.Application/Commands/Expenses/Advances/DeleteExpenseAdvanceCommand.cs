using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Services;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Advances;

public record DeleteExpenseAdvanceCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteExpenseAdvanceCommandHandler : IRequestHandler<DeleteExpenseAdvanceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvanceRepository _expenseAdvanceRepository;
    private readonly IExpenseAdvancePermissionService _permissionService;

    public DeleteExpenseAdvanceCommandHandler(
        IUserDataResolver userDataResolver,
        IExpenseAdvanceRepository expenseAdvanceRepository, 
        IS3Service s3Service,
        IExpenseAdvancePermissionService permissionService)
    {
        _userDataResolver = userDataResolver;
        _expenseAdvanceRepository = expenseAdvanceRepository;
        _s3Service = s3Service;
        _permissionService = permissionService;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteExpenseAdvanceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseAdvanceRepository.GetAsync(new GetExpenseAdvanceByIdSpec(request.Id),
                cancellationToken);

        // Sprawdź uprawnienia do edycji ewidencji tego pracownika
        var hasPermission = await _permissionService.HasPermissionAsync(
            userId, 
            entity.EmployeeId, 
            ExpenseAdvancePermissionType.Edit,
            cancellationToken);

        if (!hasPermission)
            throw DomainException.Forbidden();

        if (entity.FilePath.IsNotEmpty())
        {
            await _s3Service.DeleteFileAsync(FileType.ExpenseAdvance, entity.FilePath);
        }

        entity.Delete(userId);
        await _expenseAdvanceRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class GetExpenseAdvanceByIdSpec : BaseSpecification<ExpenseAdvanceEntity>,
    ISingleResultSpecification<ExpenseAdvanceEntity>
{
    public GetExpenseAdvanceByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(ea => ea.Id == id);
    }
}