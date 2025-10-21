using FarmsManager.Application.Models.Notifications;

namespace FarmsManager.Application.Helpers;

public static class PriorityCalculator
{
    /// <summary>
    /// Oblicza priorytet na podstawie terminu płatności i daty zapłaty
    /// </summary>
    /// <param name="dueDate">Termin płatności</param>
    /// <param name="paymentDate">Data zapłaty (jeśli już zapłacono)</param>
    /// <returns>Priorytet lub null jeśli nie ma priorytetu</returns>
    public static NotificationPriority? CalculatePriority(DateOnly? dueDate, DateOnly? paymentDate)
    {
        // Jeśli już zapłacono, nie ma priorytetu
        if (paymentDate.HasValue)
            return null;

        // Jeśli nie ma terminu, nie ma priorytetu
        if (!dueDate.HasValue)
            return null;

        var now = DateOnly.FromDateTime(DateTime.Now);
        var daysUntilDue = dueDate.Value.DayNumber - now.DayNumber;

        // Przeterminowane (czerwone)
        if (daysUntilDue < 0)
            return NotificationPriority.High;

        // Wkrótce termin - do 7 dni (żółte)
        if (daysUntilDue <= 7)
            return NotificationPriority.Medium;

        // Ostrzeżenie - 8-14 dni (niebieskie)
        if (daysUntilDue <= 14)
            return NotificationPriority.Low;

        // Daleki termin - brak priorytetu
        return null;
    }

    /// <summary>
    /// Oblicza priorytet na podstawie terminu płatności i daty zapłaty (DateTime)
    /// </summary>
    public static NotificationPriority? CalculatePriority(DateOnly? dueDate, DateTime? paymentDateUtc)
    {
        DateOnly? paymentDate = paymentDateUtc.HasValue 
            ? DateOnly.FromDateTime(paymentDateUtc.Value) 
            : null;
        
        return CalculatePriority(dueDate, paymentDate);
    }
}
