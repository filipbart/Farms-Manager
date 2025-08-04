using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public class AddNewCycleCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public int Identifier { get; init; }
    public int Year { get; init; }
}

public class AddNewCycleCommandHandler : IRequestHandler<AddNewCycleCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;

    public AddNewCycleCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddNewCycleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);

        if (request.Identifier > 6)
        {
            throw new Exception("Przekroczono limit cykli na rok");
        }

        var newCycle = CycleEntity.CreateNew(request.Identifier, request.Year, farm.Id, userId);
        farm.SetLatestCycle(newCycle);
        await _farmRepository.UpdateAsync(farm, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public class AddNewCycleCommandValidator : AbstractValidator<AddNewCycleCommand>
{
    public AddNewCycleCommandValidator()
    {
        RuleFor(t => t.Identifier).GreaterThanOrEqualTo(1);
        RuleFor(t => t.Year).Must(t =>
        {
            var yearNow = DateTime.Now.Year;
            return t == yearNow;
        }).WithMessage("Rok musi być równy bieżącemu rokowi");
    }
}