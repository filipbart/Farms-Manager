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

namespace FarmsManager.Application.Queries.Sales.Invoices;

public class
    GetSalesInvoicesQueryHandler : IRequestHandler<GetSalesInvoicesQuery, BaseResponse<GetSalesInvoicesQueryResponse>>
{
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetSalesInvoicesQueryHandler(ISaleInvoiceRepository saleInvoiceRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _saleInvoiceRepository = saleInvoiceRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetSalesInvoicesQueryResponse>> Handle(GetSalesInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _saleInvoiceRepository.ListAsync<SalesInvoiceRowDto>(
            new GetAllSalesInvoicesSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);

        var count = await _saleInvoiceRepository.CountAsync(
            new GetAllSalesInvoicesSpec(request.Filters, false, accessibleFarmIds, isAdmin), cancellationToken);

        return BaseResponse.CreateResponse(new GetSalesInvoicesQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class SalesInvoiceProfile : Profile
{
    public SalesInvoiceProfile()
    {
        CreateMap<SaleInvoiceEntity, SalesInvoiceRowDto>()
            .ForMember(m => m.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.SlaughterhouseName, opt => opt.MapFrom(t => t.Slaughterhouse.Name))
            .ForMember(m => m.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(m => m.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(m => m.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}