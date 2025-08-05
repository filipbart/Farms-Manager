namespace FarmsManager.Application.Models.Notifications;

public class NotificationData
{
    public NotificationInfo SalesInvoices { get; init; }
    public NotificationInfo FeedDeliveries { get; init; }
    public NotificationInfo Employees { get; init; }
}

public record NotificationInfo
{
    public string Count { get; init; }
    public NotificationType Type { get; init; }
}