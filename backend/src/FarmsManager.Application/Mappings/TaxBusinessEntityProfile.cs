using AutoMapper;
using FarmsManager.Application.Queries.TaxBusinessEntities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class TaxBusinessEntityProfile : Profile
{
    public TaxBusinessEntityProfile()
    {
        CreateMap<TaxBusinessEntity, TaxBusinessEntityRowDto>()
            .ForMember(d => d.HasKSeFToken, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.KSeFToken)));
    }
}
