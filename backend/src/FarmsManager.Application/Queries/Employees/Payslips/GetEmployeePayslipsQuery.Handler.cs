using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public class
    GetEmployeePayslipsQueryHandler : IRequestHandler<GetEmployeePayslipsQuery,
    BaseResponse<GetEmployeePayslipsQueryResponse>>
{
    private readonly IEmployeePayslipRepository _employeePayslipRepository;

    public GetEmployeePayslipsQueryHandler(IEmployeePayslipRepository employeePayslipRepository)
    {
        _employeePayslipRepository = employeePayslipRepository;
    }

    public async Task<BaseResponse<GetEmployeePayslipsQueryResponse>> Handle(GetEmployeePayslipsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _employeePayslipRepository.ListAsync<EmployeePayslipRowDto>(
            new GetAllEmployeePayslipsSpec(request.Filters, true), cancellationToken);

        var count = await _employeePayslipRepository.CountAsync(
            new GetAllEmployeePayslipsSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetEmployeePayslipsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class EmployeePayslipProfile : Profile
{
    public EmployeePayslipProfile()
    {
        CreateMap<EmployeePayslipEntity, EmployeePayslipRowDto>()
            .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
            .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.CycleText, opt => opt.MapFrom(src => $"{src.Cycle.Identifier}/{src.Cycle.Year}"));
    }
}