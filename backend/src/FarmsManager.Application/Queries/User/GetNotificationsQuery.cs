using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Notifications;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
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
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;

    private const int DaysForMediumLevel = 3;
    private const int AccountingDaysForLowLevel = 14;
    private const int AccountingDaysForMediumLevel = 8;
    private const int AccountingDaysForHighLevel = 4;

    public GetNotificationsQueryHandler(IUserDataResolver userDataResolver,
        IEmployeeRepository employeeRepository, ISaleInvoiceRepository saleInvoiceRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IEmployeeReminderRepository employeeReminderRepository,
        IUserRepository userRepository, IKSeFInvoiceRepository ksefInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _employeeReminderRepository = employeeReminderRepository;
        _userRepository = userRepository;
        _ksefInvoiceRepository = ksefInvoiceRepository;
    }

    public async Task<BaseResponse<GetNotificationsQueryResponse>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var farmIds = user.NotificationFarmIds;

        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);

        var notificationData = new NotificationData();

        // 1. Faktury Sprzedażowe
        var salesInvoices =
            await _saleInvoiceRepository.ListAsync(
                new GetOverdueAndUpcomingSaleInvoicesSpec(sevenDaysFromNow, farmIds),
                cancellationToken);
        ProcessNotifications(salesInvoices, d => d.DueDate, notificationData.SalesInvoices, now);

        // 2. Dostawy Pasz
        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(
                new GetOverdueAndUpcomingFeedInvoicesSpec(sevenDaysFromNow, farmIds),
                cancellationToken);
        ProcessNotifications(feedInvoices, d => d.DueDate, notificationData.FeedDeliveries, now);

        // 3. Przypomnienia i umowy pracowników
        var employeeReminders =
            await _employeeReminderRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesRemindersSpec(now, farmIds),
                cancellationToken);
        var expiringContracts =
            await _employeeRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesContractSpec(sevenDaysFromNow, farmIds),
                cancellationToken);

        // Łączymy logikę dla przypomnień i umów
        var employeeNotificationItems = employeeReminders.Select(er => er.DueDate).ToList();
        employeeNotificationItems.AddRange(expiringContracts.Select(ec => ec.EndDate!.Value));

        ProcessNotifications(employeeNotificationItems, date => date, notificationData.Employees, now);

        // 4. Faktury Księgowe (z nowymi regułami priorytetów)
        var fourteenDaysFromNow = now.AddDays(AccountingDaysForLowLevel);
        var accountingInvoices =
            await _ksefInvoiceRepository.ListAsync(
                new GetOverdueAndUpcomingAccountingInvoicesSpec(fourteenDaysFromNow, userId, farmIds),
                cancellationToken);
        ProcessAccountingNotifications(accountingInvoices.Where(i => i.PaymentDueDate.HasValue).Select(i => i.PaymentDueDate!.Value), notificationData.AccountingInvoices, now);

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

    private void ProcessAccountingNotifications(IEnumerable<DateOnly> dueDates, NotificationInfo notificationInfo, DateOnly now)
    {
        foreach (var dueDate in dueDates)
        {
            var daysUntilDue = dueDate.DayNumber - now.DayNumber;

            // Skip if 14 or more days until due (no alert)
            if (daysUntilDue >= AccountingDaysForLowLevel)
                continue;

            notificationInfo.Count++;

            var currentPriority = GetAccountingPriority(daysUntilDue);

            if (currentPriority > notificationInfo.Priority)
            {
                notificationInfo.Priority = currentPriority;
            }
        }
    }

    private static NotificationPriority GetAccountingPriority(int daysUntilDue)
    {
        // Po terminie lub 1-3 dni do terminu -> czerwony (High)
        if (daysUntilDue <= 3)
        {
            return NotificationPriority.High;
        }

        // 4-7 dni do terminu -> pomarańczowy (Medium)
        if (daysUntilDue <= 7)
        {
            return NotificationPriority.Medium;
        }

        // 8-13 dni do terminu -> żółty (Low)
        return NotificationPriority.Low;
    }
}

public sealed class GetOverdueAndUpcomingSaleInvoicesSpec : BaseSpecification<SaleInvoiceEntity>
{
    public GetOverdueAndUpcomingSaleInvoicesSpec(DateOnly date, List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        Query.Where(t => t.PaymentDate.HasValue == false);
        Query.Where(t => t.DueDate <= date);
    }
}

public sealed class GetOverdueAndUpcomingFeedInvoicesSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetOverdueAndUpcomingFeedInvoicesSpec(DateOnly date, List<Guid> accessibleFarmIds)
    {
        EnsureExists();

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        Query.Where(t => t.PaymentDateUtc.HasValue == false);
        Query.Where(t => t.DueDate <= date);
    }
}

public sealed class GetOverdueAndUpcomingEmployeesRemindersSpec : BaseSpecification<EmployeeReminderEntity>
{
    public GetOverdueAndUpcomingEmployeesRemindersSpec(DateOnly today, List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.Employee.FarmId));

        Query.Where(t => today >= t.DueDate.AddDays(-t.DaysToRemind));
    }
}

public sealed class GetOverdueAndUpcomingEmployeesContractSpec : BaseSpecification<EmployeeEntity>
{
    public GetOverdueAndUpcomingEmployeesContractSpec(DateOnly date, List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        Query.Where(t => t.EndDate.HasValue && t.EndDate.Value <= date);
    }
}