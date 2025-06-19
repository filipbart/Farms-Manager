using AutoMapper;
using FarmsManager.Application.Queries.User;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;

namespace FarmsManager.Application.Mappings.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, MeQueryResponse>();
    }
}