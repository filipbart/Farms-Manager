using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
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

    public GetPayslipsFarmsQueryHandler(IFarmRepository farmRepository)
    {
        _farmRepository = farmRepository;
    }

    public async Task<BaseResponse<GetPayslipsFarmsQueryResponse>> Handle(GetPayslipsFarmsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _farmRepository.ListAsync<FarmPayslipRowModel>(new GetAllFarmsSpec(), cancellationToken);
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
            .ForMember(m => m.Cycle, opt => opt.MapFrom(t => t.ActiveCycle));

        CreateMap<CycleEntity, CyclePayslipModel>();
        CreateMap<EmployeeEntity, EmployeeFarmPayslipModel>();
    }
}