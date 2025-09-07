using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Slaughterhouses;

public record SlaughterhouseRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public string ProducerNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public record GetAllSlaughterhousesQuery : IRequest<BaseResponse<GetAllSlaughterhousesQueryResponse>>;

public class GetAllSlaughterhousesQueryResponse : PaginationModel<SlaughterhouseRowDto>;

public class
    GetAllSlaughterhousesQueryHandler : IRequestHandler<GetAllSlaughterhousesQuery,
    BaseResponse<GetAllSlaughterhousesQueryResponse>>
{
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;

    public GetAllSlaughterhousesQueryHandler(ISlaughterhouseRepository slaughterhouseRepository)
    {
        _slaughterhouseRepository = slaughterhouseRepository;
    }


    public async Task<BaseResponse<GetAllSlaughterhousesQueryResponse>> Handle(GetAllSlaughterhousesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _slaughterhouseRepository.ListAsync<SlaughterhouseRowDto>(new GetAllSlaughterhousesSpec(),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetAllSlaughterhousesQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public sealed class GetAllSlaughterhousesSpec : BaseSpecification<SlaughterhouseEntity>
{
    public GetAllSlaughterhousesSpec()
    {
        EnsureExists();
        Query.OrderBy(t => t.Name);
    }
}

public class SlaughterhouseProfile : Profile
{
    public SlaughterhouseProfile()
    {
        CreateMap<SlaughterhouseEntity, SlaughterhouseRowDto>();
    }
}