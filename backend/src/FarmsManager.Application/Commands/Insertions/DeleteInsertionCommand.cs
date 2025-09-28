using FarmsManager.Application.Commands.ProductionData.Weighings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record DeleteInsertionCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteInsertionCommandHandler : IRequestHandler<DeleteInsertionCommand, EmptyBaseResponse>
{
    private readonly IInsertionRepository _insertionRepository;
    private readonly IProductionDataWeighingRepository _productionDataWeighingRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteInsertionCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository,
        IProductionDataWeighingRepository productionDataWeighingRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
        _productionDataWeighingRepository = productionDataWeighingRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteInsertionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var insertion = await _insertionRepository.GetAsync(new InsertionByIdSpec(request.Id), cancellationToken);

        if (insertion == null)
            throw new Exception("Wstawienie nie zostało znalezione.");

        var weighingSpec = new GetWeighingByKeysSpec(insertion.FarmId, insertion.CycleId, insertion.HenhouseId,
            insertion.HatcheryId);
        var weighing = await _productionDataWeighingRepository.FirstOrDefaultAsync(weighingSpec, cancellationToken);

        if (weighing != null)
        {
            weighing.Delete(userId);
            await _productionDataWeighingRepository.UpdateAsync(weighing, cancellationToken);
        }

        insertion.Delete(userId);
        await _insertionRepository.UpdateAsync(insertion, cancellationToken);

        return new EmptyBaseResponse();
    }
}