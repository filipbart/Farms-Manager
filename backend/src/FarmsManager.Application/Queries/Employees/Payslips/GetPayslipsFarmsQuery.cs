using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public record FarmPayslipRowModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public CyclePayslipModel Cycle { get; init; }
    public List<EmployeeFarmPayslipModel> Employees { get; init; }
}

public record CyclePayslipModel
{
    public Guid Id { get; init; }
    public int Identifier { get; init; }
    public int Year { get; init; }
}

public record EmployeeFarmPayslipModel
{
    public Guid Id { get; init; }
    public string FullName { get; init; }
    public decimal Salary { get; init; }
}

public class GetPayslipsFarmsQueryResponse : PaginationModel<FarmPayslipRowModel>;

public record GetPayslipsFarmsQuery : IRequest<BaseResponse<GetPayslipsFarmsQueryResponse>>;

public class
    GetPayslipsFarmsQueryHandler : IRequestHandler<GetPayslipsFarmsQuery, BaseResponse<GetPayslipsFarmsQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetPayslipsFarmsQueryHandler(IFarmRepository farmRepository, IUserRepository userRepository,
        IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetPayslipsFarmsQueryResponse>> Handle(GetPayslipsFarmsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var items = await _farmRepository.ListAsync<FarmPayslipRowModel>(new GetAllFarmsSpec(accessibleFarmIds),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetPayslipsFarmsQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public class PayslipsFarmsProfile : Profile
{
    public PayslipsFarmsProfile()
    {
        CreateMap<FarmEntity, FarmPayslipRowModel>()
            .ForMember(m => m.Cycle, opt => opt.MapFrom(t => t.ActiveCycle))
            .ForMember(m => m.Employees,
                opt => opt.MapFrom(t =>
                    t.Employees.Where(e => e.DateDeletedUtc.HasValue == false && e.Status == EmployeeStatus.Active)));

        CreateMap<CycleEntity, CyclePayslipModel>();
        CreateMap<EmployeeEntity, EmployeeFarmPayslipModel>();
    }
}