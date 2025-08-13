using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, BaseResponse<GetEmployeesQueryResponse>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetEmployeesQueryResponse>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _employeeRepository.ListAsync(
            new GetAllEmployeesSpec(request.Filters, true), cancellationToken);

        var count = await _employeeRepository.CountAsync(
            new GetAllEmployeesSpec(request.Filters, false), cancellationToken);
        
        var items = _mapper.Map<List<EmployeeRowDto>>(data);

        return BaseResponse.CreateResponse(new GetEmployeesQueryResponse
        {
            TotalRows = count,
            Items = items
        });
    }
}

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<EmployeeEntity, EmployeeRowDto>()
            .ForMember(m => m.Files, opt => opt.MapFrom(t => t.Files.Where(f => f.DateDeletedUtc.HasValue == false)))
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.UpcomingDeadline, opt => opt.MapFrom((src, dest) =>
            {
                var hasUpcomingEndDate = src.EndDate.HasValue &&
                                         src.EndDate.Value >= DateOnly.FromDateTime(DateTime.Today) &&
                                         src.EndDate.Value <= DateOnly.FromDateTime(DateTime.Today.AddDays(7));

                var hasUpcomingReminder = false;
                var upcomingRemind = src.Reminders?.MinBy(r => r.DueDate);

                if (upcomingRemind != null)
                {
                    hasUpcomingReminder = upcomingRemind.DueDate <=
                                          DateOnly.FromDateTime(DateTime.Today.AddDays(upcomingRemind.DaysToRemind));
                }

                return hasUpcomingEndDate || hasUpcomingReminder;
            }));

        CreateMap<EmployeeFileEntity, EmployeeFileDto>();
    }
}