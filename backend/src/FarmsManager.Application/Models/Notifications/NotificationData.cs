namespace FarmsManager.Application.Models.Notifications;

public class NotificationData
{
    public NotificationInfo SalesInvoices { get; set; } = new();
    public NotificationInfo FeedDeliveries { get; set; } = new();
    public NotificationInfo Employees { get; set; } = new();
}

public record NotificationInfo
{
    public int Count { get; set; }
    public NotificationPriority Priority { get; set; }
}