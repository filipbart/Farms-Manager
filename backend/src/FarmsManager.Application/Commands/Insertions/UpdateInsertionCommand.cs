using FarmsManager.Application.Commands.ProductionData.Weighings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record UpdateInsertionCommandDto
{
    public Guid CycleId { get; init; }

    /// <summary>
    /// Ilość sztuk
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Data wstawienia
    /// </summary>
    public DateOnly InsertionDate { get; init; }

    /// <summary>
    /// Średnia masa ciała
    /// </summary>
    public decimal BodyWeight { get; init; }

    /// <summary>
    /// Komentarz
    /// </summary>
    public string Comment { get; init; }
}

public record UpdateInsertionCommand(Guid Id, UpdateInsertionCommandDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateInsertionCommandHandler : IRequestHandler<UpdateInsertionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInsertionRepository _insertionRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IProductionDataWeighingRepository _productionDataWeighingRepository;

    public UpdateInsertionCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository,
        ICycleRepository cycleRepository, IProductionDataWeighingRepository productionDataWeighingRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
        _cycleRepository = cycleRepository;
        _productionDataWeighingRepository = productionDataWeighingRepository;
    }


    public async Task<EmptyBaseResponse> Handle(UpdateInsertionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var insertion = await _insertionRepository.GetAsync(new InsertionByIdSpec(request.Id), cancellationToken);

        var existingWeighing = await _productionDataWeighingRepository.FirstOrDefaultAsync(
            new GetWeighingByKeysSpec(insertion.FarmId, insertion.CycleId, insertion.HenhouseId, insertion.HatcheryId),
            cancellationToken);

        if (existingWeighing is not null)
        {
            existingWeighing.UpdateWeighing(1, 0, request.Data.BodyWeight);
            if (insertion.CycleId != request.Data.CycleId)
            {
                existingWeighing.SetCycle(request.Data.CycleId);
            }

            await _productionDataWeighingRepository.UpdateAsync(existingWeighing, cancellationToken);
        }

        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        if (insertion.CycleId != cycle.Id)
        {
            insertion.SetCycle(cycle.Id);
        }

        insertion.UpdateData(request.Data.InsertionDate, request.Data.Quantity, request.Data.BodyWeight, request.Data.Comment);
        insertion.SetModified(userId);
        await _insertionRepository.UpdateAsync(insertion, cancellationToken);

        return new EmptyBaseResponse();
    }
}