using FarmsManager.Application.Models.Notifications;

namespace FarmsManager.Application.Helpers;

public static class PriorityCalculator
{
    /// <summary>
    /// Oblicza priorytet na podstawie terminu płatności i daty zapłaty
    /// </summary>
    /// <param name="dueDate">Termin płatności</param>
    /// <param name="paymentDate">Data zapłaty (jeśli już zapłacono)</param>
    /// <param name="isCorrection">Czy to jest korekta (korekty nie dostają kolorów)</param>
    /// <returns>Priorytet lub null jeśli nie ma priorytetu</returns>
    public static NotificationPriority? CalculatePriority(DateOnly? dueDate, DateOnly? paymentDate,
        bool isCorrection = false)
    {
        // Korekty nie dostają kolorów
        if (isCorrection)
            return null;

        // Jeśli już zapłacono, nie ma priorytetu
        if (paymentDate.HasValue)
            return null;

        // Jeśli nie ma terminu, nie ma priorytetu
        if (!dueDate.HasValue)
            return null;

        var now = DateOnly.FromDateTime(DateTime.Now);
        var daysUntilDue = dueDate.Value.DayNumber - now.DayNumber;

        return daysUntilDue switch
        {
            // Dzień terminu lub po terminie (0 dni lub mniej) - czerwony
            <= 0 => NotificationPriority.High,
            // 1-3 dni do końca - żółty
            <= 3 => NotificationPriority.Medium,
            // 4-7 dni do końca - niebieski
            <= 7 => NotificationPriority.Low,
            _ => null
        };

        // Więcej niż 7 dni - bez koloru
    }

    /// <summary>
    /// Oblicza priorytet na podstawie terminu płatności i daty zapłaty (DateTime)
    /// </summary>
    /// <param name="dueDate">Termin płatności</param>
    /// <param name="paymentDateUtc">Data zapłaty UTC (jeśli już zapłacono)</param>
    /// <param name="isCorrection">Czy to jest korekta (korekty nie dostają kolorów)</param>
    /// <returns>Priorytet lub null jeśli nie ma priorytetu</returns>
    public static NotificationPriority? CalculatePriority(DateOnly? dueDate, DateTime? paymentDateUtc,
        bool isCorrection = false)
    {
        DateOnly? paymentDate = paymentDateUtc.HasValue
            ? DateOnly.FromDateTime(paymentDateUtc.Value)
            : null;

        return CalculatePriority(dueDate, paymentDate, isCorrection);
    }
}