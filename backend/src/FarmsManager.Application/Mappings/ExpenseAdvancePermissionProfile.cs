using AutoMapper;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class ExpenseAdvancePermissionProfile : Profile
{
    public ExpenseAdvancePermissionProfile()
    {
        CreateMap<ExpenseAdvancePermissionEntity, ExpenseAdvancePermissionDto>()
            .ForMember(dest => dest.ExpenseAdvanceId, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.DateCreatedUtc))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.DateModifiedUtc));

        CreateMap<EmployeeEntity, ExpenseAdvanceEntityDto>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Position));
    }
}
