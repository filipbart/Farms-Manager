using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries;

public record HatcheryRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public record GetAllHatcheriesQuery : IRequest<BaseResponse<GetAllHatcheriesQueryResponse>>;

public class GetAllHatcheriesQueryResponse : PaginationModel<HatcheryRowDto>;

public class
    GetAllHatcheriesQueryHandler : IRequestHandler<GetAllHatcheriesQuery, BaseResponse<GetAllHatcheriesQueryResponse>>
{
    private readonly IHatcheryRepository _hatcheryRepository;

    public GetAllHatcheriesQueryHandler(IHatcheryRepository hatcheryRepository)
    {
        _hatcheryRepository = hatcheryRepository;
    }

    public async Task<BaseResponse<GetAllHatcheriesQueryResponse>> Handle(GetAllHatcheriesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _hatcheryRepository.ListAsync<HatcheryRowDto>(new GetAllHatcheriesSpec(), cancellationToken);
        return BaseResponse.CreateResponse(new GetAllHatcheriesQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public sealed class GetAllHatcheriesSpec : BaseSpecification<HatcheryEntity>
{
    public GetAllHatcheriesSpec()
    {
        EnsureExists();
    }
}