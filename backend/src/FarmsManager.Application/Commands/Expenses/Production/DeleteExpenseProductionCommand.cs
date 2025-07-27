using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record DeleteExpenseProductionCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteExpenseProductionCommandHandler : IRequestHandler<DeleteExpenseProductionCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseProductionRepository _expenseProductionRepository;

    public DeleteExpenseProductionCommandHandler(IUserDataResolver userDataResolver,
        IExpenseProductionRepository expenseProductionRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _expenseProductionRepository = expenseProductionRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteExpenseProductionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseProductionRepository.GetAsync(new GetExpenseProductionByIdSpec(request.Id),
                cancellationToken);

        if (entity.FilePath.IsNotEmpty())
        {
            await _s3Service.DeleteFileAsync(FileType.ExpenseProduction, entity.FilePath);
        }

        entity.Delete(userId);
        await _expenseProductionRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}