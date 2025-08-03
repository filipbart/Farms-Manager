using AutoMapper;
using FarmsManager.Application.Extensions;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Queries.Sales.ExportFile;

public class SaleExportFileProfile : Profile
{
    public SaleExportFileProfile()
    {
        CreateMap<SaleEntity, SalesExportFileModel>()
            .ForMember(m => m.Type, opt => opt.MapFrom(s => s.Type.GetDescription()))
            .ForMember(m => m.Farm, opt => opt.MapFrom(s => s.Farm.Name))
            .ForMember(m => m.Cycle, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(m => m.Slaughterhouse, opt => opt.MapFrom(t => t.Slaughterhouse.Name))
            .ForMember(m => m.Henhouse, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(m => m.DateCreated, opt => opt.MapFrom(t => t.DateCreatedUtc.ToLocalTime()))
            .ForMember(m => m.OtherExtras, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.OtherExtras = src.OtherExtras?.Select(o => new SaleOtherExtrasFileModel
                {
                    Name = o.Name,
                    Value = o.Value
                }).ToList();
            });
    }
}