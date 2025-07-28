using AutoMapper;
using FarmsManager.Application.Queries.User;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;

namespace FarmsManager.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, MeQueryResponse>();
    }
}