using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Models.Employees;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public record EmployeeDetailsQueryResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public string Position { get; init; }
    public string ContractType { get; init; }
    public decimal Salary { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public EmployeeStatus Status { get; init; }
    public string StatusDesc => Status.GetDescription();
    public string Comment { get; init; }
    public List<EmployeeFileDto> Files { get; init; }
    public List<EmployeeReminderDto> Reminders { get; init; }
}

public record GetEmployeeDetailsQuery(Guid EmployeeId) : IRequest<BaseResponse<EmployeeDetailsQueryResponse>>;

public class
    GetEmployeeDetailsQueryHandler : IRequestHandler<GetEmployeeDetailsQuery,
    BaseResponse<EmployeeDetailsQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeDetailsQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<BaseResponse<EmployeeDetailsQueryResponse>> Handle(GetEmployeeDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository.GetAsync<EmployeeDetailsQueryResponse>(
                new GetEmployeeByIdWithFilesSpec(request.EmployeeId), cancellationToken);

        return BaseResponse.CreateResponse(employee);
    }
}

public class EmployeeDetailsProfile : Profile
{
    public EmployeeDetailsProfile()
    {
        CreateMap<EmployeeEntity, EmployeeDetailsQueryResponse>()
            .ForMember(m => m.Files, opt => opt.MapFrom(m => m.Files.Where(f => f.DateDeletedUtc.HasValue == false)))
            .ForMember(m => m.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
            .ForMember(m => m.Reminders,
                opt => opt.MapFrom(t => t.Reminders.Where(r => r.DateDeletedUtc.HasValue == false)));

        CreateMap<EmployeeReminderEntity, EmployeeReminderDto>();
    }
}