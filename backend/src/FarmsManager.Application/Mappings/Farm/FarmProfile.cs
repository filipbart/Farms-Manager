using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings.Farm;

public class FarmProfile : Profile
{
    public FarmProfile()
    {
        CreateMap<FarmEntity, FarmRowDto>()
            .ForMember(m => m.HenHousesCount,
                opt => opt.MapFrom(t => t.Henhouses.Count(h => h.DateDeletedUtc.HasValue == false)))
            .ForMember(m => m.Henhouses,
                opt => opt.MapFrom(t => t.Henhouses.Where(h => h.DateDeletedUtc.HasValue == false)));
    }
}

public class CycleProfile : Profile
{
    public CycleProfile()
    {
        CreateMap<CycleEntity, FarmLatestCycleDto>();
    }
}