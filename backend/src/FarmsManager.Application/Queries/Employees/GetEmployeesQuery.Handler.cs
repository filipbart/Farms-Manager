using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Employees;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, BaseResponse<GetEmployeesQueryResponse>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetEmployeesQueryResponse>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _employeeRepository.ListAsync(
            new GetAllEmployeesSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);

        var count = await _employeeRepository.CountAsync(
            new GetAllEmployeesSpec(request.Filters, false, accessibleFarmIds, isAdmin), cancellationToken);

        var items = _mapper.Map<List<EmployeeRowDto>>(data).ClearAdminData(isAdmin);

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
            .ForMember(m => m.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(m => m.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(m => m.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null))
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