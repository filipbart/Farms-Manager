using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public class GetProductionDataWeighingsQueryHandler : IRequestHandler<GetProductionDataWeighingsQuery,
    BaseResponse<GetProductionDataWeighingsQueryResponse>>
{
    private readonly IProductionDataWeighingRepository _weighingRepository;
    private readonly IProductionDataWeightStandardRepository _productionDataWeightStandardRepository;
    private readonly IMapper _mapper;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetProductionDataWeighingsQueryHandler(
        IProductionDataWeighingRepository weighingRepository,
        IMapper mapper, IProductionDataWeightStandardRepository productionDataWeightStandardRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _weighingRepository = weighingRepository;
        _mapper = mapper;
        _productionDataWeightStandardRepository = productionDataWeightStandardRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetProductionDataWeighingsQueryResponse>> Handle(
        GetProductionDataWeighingsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var spec = new GetAllProductionDataWeighingsSpec(request.Filters, true, accessibleFarmIds);
        var countSpec = new GetAllProductionDataWeighingsSpec(request.Filters, false, accessibleFarmIds);

        var weighings = await _weighingRepository.ListAsync(spec, cancellationToken);
        var totalRows = await _weighingRepository.CountAsync(countSpec, cancellationToken);

        var standards = await _productionDataWeightStandardRepository.ListAsync(cancellationToken);
        var standardsDict = standards.ToDictionary(s => s.Day, s => s.Weight);

        var data = _mapper.Map<List<ProductionDataWeighingRowDto>>(weighings);
        foreach (var row in data)
        {
            row.Weighing1Deviation = CalculateDeviation(row.Weighing1Day, row.Weighing1Weight, standardsDict);
            row.Weighing2Deviation = CalculateDeviation(row.Weighing2Day, row.Weighing2Weight, standardsDict);
            row.Weighing3Deviation = CalculateDeviation(row.Weighing3Day, row.Weighing3Weight, standardsDict);
            row.Weighing4Deviation = CalculateDeviation(row.Weighing4Day, row.Weighing4Weight, standardsDict);
            row.Weighing5Deviation = CalculateDeviation(row.Weighing5Day, row.Weighing5Weight, standardsDict);
        }

        return BaseResponse.CreateResponse(new GetProductionDataWeighingsQueryResponse
        {
            TotalRows = totalRows,
            Items = data
        });
    }

    private static decimal? CalculateDeviation(int? day, decimal? weight, IReadOnlyDictionary<int, decimal> standards)
    {
        if (day is null || weight is null || !standards.TryGetValue(day.Value, out var standardWeight))
        {
            return null;
        }

        return weight.Value - standardWeight;
    }
}

public class ProductionDataWeighingProfile : Profile
{
    public ProductionDataWeighingProfile()
    {
        CreateMap<ProductionDataWeighingEntity, ProductionDataWeighingRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.HatcheryName, opt => opt.MapFrom(t => t.Hatchery.Name));
    }
}