using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Insertions;

public class GetInsertionsQueryHandler : IRequestHandler<GetInsertionsQuery, BaseResponse<GetInsertionsQueryResponse>>
{
    private readonly IInsertionRepository _insertionRepository;

    public GetInsertionsQueryHandler(IInsertionRepository insertionRepository)
    {
        _insertionRepository = insertionRepository;
    }

    public async Task<BaseResponse<GetInsertionsQueryResponse>> Handle(GetInsertionsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _insertionRepository.ListAsync<InsertionRowDto>(
            new GetAllInsertionsSpec(request.Filters, true), cancellationToken);
        var count = await _insertionRepository.CountAsync(new GetAllInsertionsSpec(request.Filters, false),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetInsertionsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class InsertionProfile : Profile
{
    public InsertionProfile()
    {
        CreateMap<InsertionEntity, InsertionRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.HatcheryName, opt => opt.MapFrom(t => t.Hatchery.Name));
    }
}