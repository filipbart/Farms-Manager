using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record WeighingEntryDto
{
    public Guid HenhouseId { get; init; }
    public Guid HatcheryId { get; init; }
    public int Day { get; init; }
    public decimal Weight { get; init; }
}

public record AddProductionDataWeighingCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public int WeighingNumber { get; init; }
    public List<WeighingEntryDto> Entries { get; init; } = [];
}

public class
    AddProductionDataWeighingCommandHandler : IRequestHandler<AddProductionDataWeighingCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataWeighingRepository _weighingRepository;

    public AddProductionDataWeighingCommandHandler(
        IUserDataResolver userDataResolver,
        IProductionDataWeighingRepository weighingRepository)
    {
        _userDataResolver = userDataResolver;
        _weighingRepository = weighingRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataWeighingCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var updatedWeighings = new List<ProductionDataWeighingEntity>();

        foreach (var entry in request.Entries)
        {
            var spec = new GetWeighingByKeysSpec(request.FarmId, request.CycleId, entry.HenhouseId, entry.HatcheryId);
            var weighing = await _weighingRepository.FirstOrDefaultAsync(spec, ct);

            if (weighing is null)
            {
                throw new Exception(
                    "Nie znaleziono rekordu ważenia. Należy najpierw dodać 'Ważenie 1' poprzez wstawienie.");
            }

            weighing.UpdateWeighing(request.WeighingNumber, entry.Day, entry.Weight);
            weighing.SetModified(userId);
            updatedWeighings.Add(weighing);
        }

        if (updatedWeighings.Count != 0)
        {
            await _weighingRepository.UpdateRangeAsync(updatedWeighings, ct);
        }

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataWeighingValidator : AbstractValidator<AddProductionDataWeighingCommand>
{
    public AddProductionDataWeighingValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(x => x.WeighingNumber).InclusiveBetween(2, 5).WithMessage("Numer ważenia musi być od 2 do 5.");
        RuleFor(t => t.Entries).NotEmpty();

        RuleForEach(t => t.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.HenhouseId).NotEmpty();
            entry.RuleFor(e => e.HatcheryId).NotEmpty();
            entry.RuleFor(e => e.Day).GreaterThanOrEqualTo(0);
            entry.RuleFor(e => e.Weight).GreaterThan(0);
        });
    }
}

public sealed class GetWeighingByKeysSpec : BaseSpecification<ProductionDataWeighingEntity>
{
    public GetWeighingByKeysSpec(Guid farmId, Guid cycleId, Guid henhouseId, Guid hatcheryId)
    {
        EnsureExists();
        Query.Where(t =>
            t.FarmId == farmId && t.CycleId == cycleId && t.HenhouseId == henhouseId && t.HatcheryId == hatcheryId);
    }
}