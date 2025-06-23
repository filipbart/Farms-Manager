using AutoMapper;
using FarmsManager.Application.Queries.Hatcheries;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Application.Mappings.Hatchery;

public class HatcheryProfile : Profile
{
    public HatcheryProfile()
    {
        CreateMap<HatcheryEntity, HatcheryRowDto>();
    }
}