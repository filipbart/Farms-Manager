using AutoMapper;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;

namespace FarmsManager.Application.Mappings.Sales;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SlaughterhouseEntity, DictModel>();
    }
}