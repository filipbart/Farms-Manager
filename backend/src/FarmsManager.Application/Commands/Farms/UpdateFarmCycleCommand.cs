using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record UpdateFarmCycleCommand(Guid FarmId, string Cycle) : IRequest<EmptyBaseResponse>;

public class UpdateFarmCycleCommandValidator : AbstractValidator<UpdateFarmCycleCommand>
{
    public UpdateFarmCycleCommandValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.Cycle).NotEmpty();
    }
}

public class UpdateFarmCycleCommandHandler : IRequestHandler<UpdateFarmCycleCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateFarmCycleCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository,
        ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFarmCycleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);

        var splittedCycle = request.Cycle.Split("-");
        var identifier = int.Parse(splittedCycle[0]);
        var year = int.Parse(splittedCycle[1]);

        var farmCycles = farm.Cycles.Where(t => t.DateDeletedUtc.HasValue == false);
        var cycle = farmCycles.FirstOrDefault(t => t.Year == year && t.Identifier == identifier);
        if (cycle == null)
        {
            cycle = CycleEntity.CreateNew(identifier, year, farm.Id, userId);
            await _cycleRepository.AddAsync(cycle, cancellationToken);
        }

        farm.SetLatestCycle(cycle);
        await _farmRepository.UpdateAsync(farm, cancellationToken);
        return new EmptyBaseResponse();
    }
}