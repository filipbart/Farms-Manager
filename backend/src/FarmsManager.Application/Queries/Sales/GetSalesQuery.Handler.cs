using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Sales;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, BaseResponse<GetSalesQueryResponse>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetSalesQueryHandler(ISaleRepository saleRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetSalesQueryResponse>> Handle(GetSalesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _saleRepository.ListAsync<SaleRowDto>(
            new GetAllSalesSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);
        var count = await _saleRepository.CountAsync(new GetAllSalesSpec(request.Filters, false, accessibleFarmIds, isAdmin),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetSalesQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleEntity, SaleRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.SlaughterhouseName, opt => opt.MapFrom(t => t.Slaughterhouse.Name));
    }
}