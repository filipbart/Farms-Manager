using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Gas;

public record GetGasContractorsQuery : IRequest<BaseResponse<GetGasContractorsQueryResponse>>;

public record GetGasContractorsQueryResponse
{
    public List<GasContractorRow> Contractors { get; set; }
}

public record GasContractorRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class
    GetGasContractorsQueryHandler : IRequestHandler<GetGasContractorsQuery,
    BaseResponse<GetGasContractorsQueryResponse>>
{
    private readonly IGasContractorRepository _gasContractorRepository;

    public GetGasContractorsQueryHandler(IGasContractorRepository gasContractorRepository)
    {
        _gasContractorRepository = gasContractorRepository;
    }

    public async Task<BaseResponse<GetGasContractorsQueryResponse>> Handle(GetGasContractorsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _gasContractorRepository.ListAsync<GasContractorRow>(new GetAllGasContractorsSpec(),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetGasContractorsQueryResponse
        {
            Contractors = items
        });
    }
}

public class GasContractorsProfile : Profile
{
    public GasContractorsProfile()
    {
        CreateMap<GasContractorEntity, GasContractorRow>();
    }
}