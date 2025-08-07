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
    private readonly IMapper _mapper;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;

    public GetEmployeePayslipsQueryHandler(IMapper mapper, IEmployeePayslipRepository employeePayslipRepository)
    {
        _mapper = mapper;
        _employeePayslipRepository = employeePayslipRepository;
    }

    public async Task<BaseResponse<GetEmployeePayslipsQueryResponse>> Handle(GetEmployeePayslipsQuery request,
        CancellationToken cancellationToken)
    {
        var paginatedSpec = new GetAllEmployeePayslipsSpec(request.Filters, true);
        var fullSpec = new GetAllEmployeePayslipsSpec(request.Filters, false);

        // Pobieranie danych dla aktualnej strony i łącznej liczby
        var data = await _employeePayslipRepository.ListAsync(paginatedSpec, cancellationToken);
        var count = await _employeePayslipRepository.CountAsync(fullSpec, cancellationToken);

        // Pobieranie wszystkich pasujących rekordów w celu obliczenia sum
        var items = _mapper.Map<List<EmployeePayslipRowDto>>(data);

        // Obliczanie agregacji
        var aggregation = new EmployeePayslipAggregationDto
        {
            BaseSalary = data.Sum(p => p.BaseSalary),
            BankTransferAmount = data.Sum(p => p.BankTransferAmount),
            BonusAmount = data.Sum(p => p.BonusAmount),
            OvertimePay = data.Sum(p => p.OvertimePay),
            OvertimeHours = data.Sum(p => p.OvertimeHours),
            Deductions = data.Sum(p => p.Deductions),
            OtherAllowances = data.Sum(p => p.OtherAllowances),
            NetPay = data.Sum(p => p.NetPay)
        };

        return BaseResponse.CreateResponse(new GetEmployeePayslipsQueryResponse
        {
            List = new PaginationModel<EmployeePayslipRowDto>
            {
                TotalRows = count,
                Items = items
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