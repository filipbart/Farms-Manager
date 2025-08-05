using System.ComponentModel;

namespace FarmsManager.Application.Models.Notifications;

public enum NotificationPriority
{
    [Description("Niski")] Low,
    [Description("Średni")] Medium,
    [Description("Wysoki")] High
}