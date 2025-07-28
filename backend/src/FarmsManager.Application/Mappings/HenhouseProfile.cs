using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings;

public class HenhouseProfile : Profile
{
    public HenhouseProfile()
    {
        CreateMap<HenhouseEntity, HenhouseRowDto>();
        CreateMap<HenhouseEntity, DictModel>();
    }
}