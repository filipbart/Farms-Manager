using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Application.Queries.Insertions.Dictionary;

public class GetDictionaryQueryProfile : Profile
{
    public GetDictionaryQueryProfile()
    {
        CreateMap<FarmEntity, FarmDictModel>()
            .ForMember(t => t.Henhouses,
                opt => opt.MapFrom(t => t.Henhouses.Where(h => h.DateDeletedUtc.HasValue == false)));
        CreateMap<HenhouseEntity, HenhouseDictModel>();
        CreateMap<HatcheryEntity, HatcheryDictModel>();
        CreateMap<CycleEntity, CycleDictModel>();
    }
}