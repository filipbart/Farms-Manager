using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SeedWork.Entities;
using FarmsManager.Domain.Aggregates.SeedWork.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ColumnsViews;

public record DeleteColumnViewCommand(Guid ColumnViewId) : IRequest<EmptyBaseResponse>;

public class DeleteColumnViewCommandHandler : IRequestHandler<DeleteColumnViewCommand, EmptyBaseResponse>
{
    private readonly IColumnViewRepository _columnViewRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteColumnViewCommandHandler(IColumnViewRepository columnViewRepository,
        IUserDataResolver userDataResolver)
    {
        _columnViewRepository = columnViewRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteColumnViewCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _columnViewRepository.GetAsync(new ColumnViewByIdSpec(request.ColumnViewId), cancellationToken);

        entity.Delete(userId);
        await _columnViewRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class ColumnViewByIdSpec : BaseSpecification<ColumnViewEntity>,
    ISingleResultSpecification<ColumnViewEntity>
{
    public ColumnViewByIdSpec(Guid columnViewId)
    {
        EnsureExists();
        Query.Where(t => t.Id == columnViewId);
    }
}

public class DeleteColumnViewCommandValidator : AbstractValidator<DeleteColumnViewCommand>
{
    public DeleteColumnViewCommandValidator()
    {
        RuleFor(x => x.ColumnViewId)
            .NotEmpty();
    }
}