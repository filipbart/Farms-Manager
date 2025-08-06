using AutoMapper;
using FarmsManager.Application.Common;
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
        var paginatedSpec = new GetAllEmployeePayslipsSpec(request.Filters, true);
        var fullSpec = new GetAllEmployeePayslipsSpec(request.Filters, false);

        // Pobieranie danych dla aktualnej strony i łącznej liczby
        var data = await _employeePayslipRepository.ListAsync<EmployeePayslipRowDto>(paginatedSpec, cancellationToken);
        var count = await _employeePayslipRepository.CountAsync(fullSpec, cancellationToken);

        // Pobieranie wszystkich pasujących rekordów w celu obliczenia sum
        var allMatchingPayslips = await _employeePayslipRepository.ListAsync(fullSpec, cancellationToken);

        // Obliczanie agregacji
        var aggregation = new EmployeePayslipAggregationDto
        {
            BaseSalary = allMatchingPayslips.Sum(p => p.BaseSalary),
            BankTransferAmount = allMatchingPayslips.Sum(p => p.BankTransferAmount),
            BonusAmount = allMatchingPayslips.Sum(p => p.BonusAmount),
            OvertimePay = allMatchingPayslips.Sum(p => p.OvertimePay),
            OvertimeHours = allMatchingPayslips.Sum(p => p.OvertimeHours),
            Deductions = allMatchingPayslips.Sum(p => p.Deductions),
            OtherAllowances = allMatchingPayslips.Sum(p => p.OtherAllowances),
            NetPay = allMatchingPayslips.Sum(p => p.NetPay)
        };

        return BaseResponse.CreateResponse(new GetEmployeePayslipsQueryResponse
        {
            List = new PaginationModel<EmployeePayslipRowDto>
            {
                TotalRows = count,
                Items = data
            },
            Aggregation = aggregation
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