using AutoMapper;
using FarmsManager.Application.Queries.User;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, MeQueryResponse>()
            .ForMember(m => m.Permissions, opt => opt.MapFrom(t => t.Permissions.Select(p => p.PermissionName)))
            .ForMember(m => m.AccessibleFarmIds, opt => opt.MapFrom(m => m.Farms.Select(t => t.FarmId)));
    }
}