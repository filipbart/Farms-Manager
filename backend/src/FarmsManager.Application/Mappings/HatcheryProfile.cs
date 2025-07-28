using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Hatcheries;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class HatcheryProfile : Profile
{
    public HatcheryProfile()
    {
        CreateMap<HatcheryEntity, HatcheryRowDto>();
        CreateMap<HatcheryEntity, DictModel>();
    }
}