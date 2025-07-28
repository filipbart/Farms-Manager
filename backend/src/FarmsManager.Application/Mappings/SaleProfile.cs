using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SlaughterhouseEntity, DictModel>();
    }
}