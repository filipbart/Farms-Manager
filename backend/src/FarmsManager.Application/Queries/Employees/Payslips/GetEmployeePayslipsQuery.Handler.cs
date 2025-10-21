using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public class
    GetEmployeePayslipsQueryHandler : IRequestHandler<GetEmployeePayslipsQuery,
    BaseResponse<GetEmployeePayslipsQueryResponse>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetEmployeePayslipsQueryHandler(IMapper mapper, IEmployeePayslipRepository employeePayslipRepository,
        IUserRepository userRepository, IUserDataResolver userDataResolver)
    {
        _mapper = mapper;
        _employeePayslipRepository = employeePayslipRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetEmployeePayslipsQueryResponse>> Handle(GetEmployeePayslipsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var paginatedSpec = new GetAllEmployeePayslipsSpec(request.Filters, true, accessibleFarmIds, isAdmin);
        var fullSpec = new GetAllEmployeePayslipsSpec(request.Filters, false, accessibleFarmIds, isAdmin);

        // Pobieranie danych dla aktualnej strony i łącznej liczby
        var data = await _employeePayslipRepository.ListAsync(paginatedSpec, cancellationToken);
        var count = await _employeePayslipRepository.CountAsync(fullSpec, cancellationToken);

        // Pobieranie wszystkich pasujących rekordów w celu obliczenia sum
        var items = _mapper.Map<List<EmployeePayslipRowDto>>(data).ClearAdminData(isAdmin);

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
            .ForMember(dest => dest.CycleText, opt => opt.MapFrom(src => $"{src.Cycle.Identifier}/{src.Cycle.Year}"))
            .ForMember(dest => dest.TotalAmount,
                opt => opt.MapFrom(src =>
                    src.BankTransferAmount + src.BonusAmount + src.OvertimePay - src.Deductions + src.OtherAllowances))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Name : null))
            .ForMember(dest => dest.ModifiedByName, opt => opt.MapFrom(src => src.Modifier != null ? src.Modifier.Name : null))
            .ForMember(dest => dest.DeletedByName, opt => opt.MapFrom(src => src.Deleter != null ? src.Deleter.Name : null));
    }
}