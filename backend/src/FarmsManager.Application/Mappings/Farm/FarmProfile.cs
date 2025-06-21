using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings.Farm;

public class FarmProfile : Profile
{
    public FarmProfile()
    {
        CreateMap<FarmEntity, FarmRowDto>()
            .ForMember(m => m.HenHousesCount, opt => opt.MapFrom(t => 2));
    }
}