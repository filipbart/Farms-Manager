using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Mappings.Henhouse;

public class HenhouseProfile : Profile
{
    public HenhouseProfile()
    {
        CreateMap<HenhouseEntity, HenhouseRowDto>();
    }
}