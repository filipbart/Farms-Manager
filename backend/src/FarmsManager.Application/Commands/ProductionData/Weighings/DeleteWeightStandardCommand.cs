using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record DeleteWeightStandardCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteWeightStandardCommandHandler : IRequestHandler<DeleteWeightStandardCommand, EmptyBaseResponse>
{
    private readonly IProductionDataWeightStandardRepository _repository;

    public DeleteWeightStandardCommandHandler(IProductionDataWeightStandardRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteWeightStandardCommand request,
        CancellationToken cancellationToken)
    {
        var standard = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (standard is not null)
        {
            await _repository.DeleteAsync(standard, cancellationToken);
        }

        return BaseResponse.EmptyResponse;
    }
}