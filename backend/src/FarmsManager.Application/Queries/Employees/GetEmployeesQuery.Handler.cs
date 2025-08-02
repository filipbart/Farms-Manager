using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, BaseResponse<GetEmployeesQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<BaseResponse<GetEmployeesQueryResponse>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _employeeRepository.ListAsync<EmployeeRowDto>(
            new GetAllEmployeesSpec(request.Filters, true), cancellationToken);

        var count = await _employeeRepository.CountAsync(
            new GetAllEmployeesSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetEmployeesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<EmployeeEntity, EmployeeRowDto>()
            .ForMember(m => m.Files, opt => opt.MapFrom(t => t.Files.Where(f => f.DateDeletedUtc.HasValue == false)))
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name));

        CreateMap<EmployeeFileEntity, EmployeeFileDto>();
    }
}