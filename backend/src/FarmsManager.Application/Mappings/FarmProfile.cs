using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings;

public class FarmProfile : Profile
{
    public FarmProfile()
    {
        CreateMap<FarmEntity, FarmRowDto>()
            .ForMember(m => m.HenHousesCount,
                opt => opt.MapFrom(t => t.Henhouses.Count(h => h.DateDeletedUtc.HasValue == false)))
            .ForMember(m => m.Henhouses,
                opt => opt.MapFrom(t =>
                    t.Henhouses.Where(h => h.DateDeletedUtc.HasValue == false).OrderBy(h => h.Name)));

        CreateMap<FarmEntity, FarmDictModel>()
            .ForMember(t => t.Henhouses,
                opt => opt.MapFrom(t =>
                    t.Henhouses.Where(h => h.DateDeletedUtc.HasValue == false).OrderBy(h => h.Name)));
    }
}