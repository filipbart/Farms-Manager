using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings;

public class CycleProfile : Profile
{
    public CycleProfile()
    {
        CreateMap<CycleEntity, CycleDto>();
        CreateMap<CycleEntity, CycleDictModel>();
    }
}