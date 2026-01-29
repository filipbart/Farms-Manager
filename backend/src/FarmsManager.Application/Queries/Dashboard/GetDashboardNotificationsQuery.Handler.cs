using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Notifications;
using FarmsManager.Application.Queries.User;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetDashboardNotificationsQueryHandler : IRequestHandler<GetDashboardNotificationsQuery,
    BaseResponse<List<DashboardNotificationItem>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;

    private const int AccountingDaysForLowLevel = 14;

    public GetDashboardNotificationsQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository, IFeedInvoiceRepository feedInvoiceRepository,
        IEmployeeRepository employeeRepository, IEmployeeReminderRepository employeeReminderRepository,
        IKSeFInvoiceRepository ksefInvoiceRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _employeeRepository = employeeRepository;
        _employeeReminderRepository = employeeReminderRepository;
        _ksefInvoiceRepository = ksefInvoiceRepository;
    }

    public async Task<BaseResponse<List<DashboardNotificationItem>>> Handle(GetDashboardNotificationsQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new List<DashboardNotificationItem>());
        }

        var accessibleFarmIds = user.AccessibleFarmIds;

        var filteredFarmIds = request.Filters.FarmId.HasValue
            ? accessibleFarmIds != null && !accessibleFarmIds.Contains(request.Filters.FarmId.Value)
                ? throw DomainException.Forbidden()
                : new List<Guid> { request.Filters.FarmId.Value }
            : accessibleFarmIds;

        var farmIds = user.NotificationFarmIds ?? filteredFarmIds;

        var notifications = await BuildDashboardNotifications(farmIds, userId, ct);

        return BaseResponse.CreateResponse(notifications);
    }

    private record NotificationSource(
        DateOnly DueDate,
        NotificationType Type,
        object Entity,
        Guid SourceId,
        int SortPriority);

    private async Task<List<DashboardNotificationItem>> BuildDashboardNotifications(List<Guid> farmIds, Guid userId,
        CancellationToken ct)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);
        var fourteenDaysFromNow = now.AddDays(AccountingDaysForLowLevel);
        const int daysForMediumPriority = 3;

        var allSources = new List<NotificationSource>();

        var salesInvoices =
            await _saleInvoiceRepository.ListAsync(new GetOverdueAndUpcomingSaleInvoicesSpec(sevenDaysFromNow, farmIds),
                ct);
        allSources.AddRange(salesInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.SaleInvoice, inv, inv.Id, 2)));

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetOverdueAndUpcomingFeedInvoicesSpec(sevenDaysFromNow, farmIds),
                ct);
        allSources.AddRange(feedInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.FeedInvoice, inv, inv.Id, 2)));

        var expiringContracts =
            await _employeeRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesContractSpec(sevenDaysFromNow, farmIds), ct);
        allSources.AddRange(expiringContracts.Select(emp =>
            new NotificationSource(emp.EndDate!.Value, NotificationType.EmployeeContract, emp, emp.Id, 1)));

        var employeeReminders =
            await _employeeReminderRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesRemindersSpec(now, sevenDaysFromNow, farmIds), ct);
        allSources.AddRange(employeeReminders.Select(rem =>
            new NotificationSource(rem.DueDate, NotificationType.EmployeeReminder, rem, rem.EmployeeId, 1)));

        // Faktury księgowe (14 dni zakres, filtrowanie wg przypisanego użytkownika)
        var accountingInvoices =
            await _ksefInvoiceRepository.ListAsync(
                new GetOverdueAndUpcomingAccountingInvoicesSpec(fourteenDaysFromNow, userId, farmIds), ct);
        allSources.AddRange(accountingInvoices
            .Where(inv => inv.PaymentDueDate.HasValue)
            .Select(inv =>
                new NotificationSource(inv.PaymentDueDate!.Value, NotificationType.AccountingInvoice, inv, inv.Id, 2)));

        var top5Notifications = allSources
            .OrderBy(s => s.SortPriority)
            .ThenBy(s => s.DueDate)
            .Take(5)
            .Select(source =>
            {
                NotificationPriority priority;
                if (source.DueDate <= now) priority = NotificationPriority.High;
                else if (source.DueDate <= now.AddDays(daysForMediumPriority)) priority = NotificationPriority.Medium;
                else priority = NotificationPriority.Low;

                var description = GenerateDescription(source, now);

                return new DashboardNotificationItem
                {
                    Description = description,
                    DueDate = source.DueDate,
                    Priority = priority,
                    Type = source.Type,
                    SourceId = source.SourceId
                };
            })
            .ToList();

        return top5Notifications;
    }

    private static string GenerateDescription(NotificationSource source, DateOnly now)
    {
        // Używamy formatowania z polskimi nazwami dni tygodnia
        var culture = new System.Globalization.CultureInfo("pl-PL");
        var dateString = source.DueDate.ToString("d MMMM yyyy", culture);
        string dayDifference;
        var days = source.DueDate.DayNumber - now.DayNumber;
        if (days < 0) dayDifference = $" (zaległe o {-days} dni)";
        else if (days == 0) dayDifference = " (dziś)";
        else if (days == 1) dayDifference = " (jutro)";
        else dayDifference = $" (za {days} dni)";

        return source.Type switch
        {
            NotificationType.SaleInvoice => $"Faktura sprzedaży: termin płatności {dateString}{dayDifference}",
            NotificationType.FeedInvoice => $"Faktura za paszę: termin płatności {dateString}{dayDifference}",
            NotificationType.EmployeeContract =>
                $"Koniec umowy dla {(source.Entity as EmployeeEntity)?.FullName}: {dateString}{dayDifference}",
            NotificationType.EmployeeReminder =>
                $"Przypomnienie '{(source.Entity as EmployeeReminderEntity)?.Title}': {dateString}{dayDifference}",
            NotificationType.AccountingInvoice =>
                $"Faktura księgowa {(source.Entity as KSeFInvoiceEntity)?.InvoiceNumber}: termin płatności {dateString}{dayDifference}",
            _ => "Nieznane powiadomienie"
        };
    }
}