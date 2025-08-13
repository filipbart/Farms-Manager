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
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.User;

public record GetNotificationsQueryResponse
{
    public NotificationData Data { get; set; }
}

public record GetNotificationsQuery : IRequest<BaseResponse<GetNotificationsQueryResponse>>;

public class
    GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, BaseResponse<GetNotificationsQueryResponse>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;

    private const int DaysForMediumLevel = 3;

    public GetNotificationsQueryHandler(IUserDataResolver userDataResolver,
        IEmployeeRepository employeeRepository, ISaleInvoiceRepository saleInvoiceRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IEmployeeReminderRepository employeeReminderRepository)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _employeeReminderRepository = employeeReminderRepository;
    }


    public async Task<BaseResponse<GetNotificationsQueryResponse>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);

        var notificationData = new NotificationData();

        // 1. Faktury Sprzedażowe
        var salesInvoices =
            await _saleInvoiceRepository.ListAsync(new GetOverdueAndUpcomingSaleInvoicesSpec(sevenDaysFromNow),
                cancellationToken);
        ProcessNotifications(salesInvoices, d => d.DueDate, notificationData.SalesInvoices, now);

        // 2. Dostawy Pasz
        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetOverdueAndUpcomingFeedInvoicesSpec(sevenDaysFromNow),
                cancellationToken);
        ProcessNotifications(feedInvoices, d => d.DueDate, notificationData.FeedDeliveries, now);

        // 3. Przypomnienia i umowy pracowników
        var employeeReminders =
            await _employeeReminderRepository.ListAsync(new GetOverdueAndUpcomingEmployeesRemindersSpec(now),
                cancellationToken);
        var expiringContracts =
            await _employeeRepository.ListAsync(new GetOverdueAndUpcomingEmployeesContractSpec(sevenDaysFromNow),
                cancellationToken);

        // Łączymy logikę dla przypomnień i umów
        var employeeNotificationItems = employeeReminders.Select(er => er.DueDate).ToList();
        employeeNotificationItems.AddRange(expiringContracts.Select(ec => ec.EndDate!.Value));

        ProcessNotifications(employeeNotificationItems, date => date, notificationData.Employees, now);

        return BaseResponse.CreateResponse(new GetNotificationsQueryResponse
        {
            Data = notificationData
        });
    }

    private static void ProcessNotifications<T>(IEnumerable<T> items, Func<T, DateOnly> dateSelector,
        NotificationInfo notificationInfo, DateOnly now)
    {
        foreach (var item in items)
        {
            var dueDate = dateSelector(item);
            notificationInfo.Count++;

            var currentPriority = GetPriority(dueDate, now);

            // Ustawiamy najwyższy znaleziony priorytet
            if (currentPriority > notificationInfo.Priority)
            {
                notificationInfo.Priority = currentPriority;
            }
        }
    }

    private static NotificationPriority GetPriority(DateOnly dueDate, DateOnly now)
    {
        if (dueDate <= now)
        {
            return NotificationPriority.High;
        }

        if (dueDate <= now.AddDays(DaysForMediumLevel))
        {
            return NotificationPriority.Medium;
        }

        return NotificationPriority.Low;
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