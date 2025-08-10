using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.SeedWork.Entities;
using FarmsManager.Domain.Aggregates.SeedWork.Enums;
using FarmsManager.Domain.Aggregates.SeedWork.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ColumnsViews;

public record AddColumnViewCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string State { get; init; }
    public ColumnViewType Type { get; init; }
}

public class AddColumnViewCommandHandler : IRequestHandler<AddColumnViewCommand, EmptyBaseResponse>
{
    private readonly IColumnViewRepository _columnViewRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddColumnViewCommandHandler(IColumnViewRepository columnViewRepository, IUserDataResolver userDataResolver)
    {
        _columnViewRepository = columnViewRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddColumnViewCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var columnView = ColumnViewEntity.CreateNew(request.Name, request.Type, request.State, userId);
        await _columnViewRepository.AddAsync(columnView, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class AddColumnViewCommandValidator : AbstractValidator<AddColumnViewCommand>
{
    public AddColumnViewCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.State)
            .NotEmpty();
    }
}