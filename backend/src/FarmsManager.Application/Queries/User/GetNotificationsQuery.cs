using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Notifications;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.User;

public record GetNotificationsQueryResponse
{
    public NotificationData Data { get; init; }
}

public record GetNotificationsQuery : IRequest<BaseResponse<GetNotificationsQueryResponse>>;

public class
    GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, BaseResponse<GetNotificationsQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;

    private const int DaysForMediumLevel = 3;

    public GetNotificationsQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IEmployeeRepository employeeRepository, ISaleInvoiceRepository saleInvoiceRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IEmployeeReminderRepository employeeReminderRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _employeeReminderRepository = employeeReminderRepository;
    }


    public async Task<BaseResponse<GetNotificationsQueryResponse>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);
    }
}

public sealed class GetOverdueAndUpcomingSaleInvoicesSpec : BaseSpecification<SaleInvoiceEntity>
{
    public GetOverdueAndUpcomingSaleInvoicesSpec(DateOnly date)
    {
        EnsureExists();
        Query.Where(t => t.PaymentDate.HasValue == false);
        Query.Where(t => t.DueDate <= date);
    }
}

public sealed class GetOverdueAndUpcomingFeedInvoicesSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetOverdueAndUpcomingFeedInvoicesSpec(DateOnly date)
    {
        EnsureExists();
        Query.Where(t => t.PaymentDateUtc.HasValue == false);
        Query.Where(t => t.DueDate <= date);
    }
}

public sealed class GetOverdueAndUpcomingEmployeesRemindersSpec : BaseSpecification<EmployeeReminderEntity>
{
    public GetOverdueAndUpcomingEmployeesRemindersSpec(DateOnly today)
    {
        EnsureExists();
        Query.Where(t => today >= t.DueDate.AddDays(-t.DaysToRemind));
    }
}

public sealed class GetOverdueAndUpcomingEmployeesContractSpec : BaseSpecification<EmployeeEntity>
{
    public GetOverdueAndUpcomingEmployeesContractSpec(DateOnly date)
    {
        EnsureExists();
        Query.Where(t => t.EndDate.HasValue && t.EndDate.Value <= date);
    }
}