using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Application.Queries.Insertions.Dictionary;

public class GetInsertionDictionaryQueryProfile : Profile
{
    public GetInsertionDictionaryQueryProfile()
    {
        CreateMap<FarmEntity, FarmDictModel>()
            .ForMember(t => t.Henhouses,
                opt => opt.MapFrom(t => t.Henhouses.Where(h => h.DateDeletedUtc.HasValue == false)));
        CreateMap<HenhouseEntity, DictModel>();
        CreateMap<HatcheryEntity, DictModel>();
        CreateMap<CycleEntity, CycleDictModel>();
    }
}