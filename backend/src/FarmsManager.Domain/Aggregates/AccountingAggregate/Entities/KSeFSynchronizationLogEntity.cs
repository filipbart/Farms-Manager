using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

/// <summary>
/// Encja przechowująca logi synchronizacji faktur z KSeF
/// </summary>
public class KSeFSynchronizationLogEntity : Entity
{
    /// <summary>
    /// Data i czas rozpoczęcia synchronizacji
    /// </summary>
    public DateTime StartedAt { get; set; }
    
    /// <summary>
    /// Data i czas zakończenia synchronizacji
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Status synchronizacji
    /// </summary>
    public KSeFSyncStatus Status { get; set; }
    
    /// <summary>
    /// Liczba pobranych faktur
    /// </summary>
    public int InvoicesDownloaded { get; set; }
    
    /// <summary>
    /// Liczba zapisanych faktur
    /// </summary>
    public int InvoicesSaved { get; set; }
    
    /// <summary>
    /// Liczba błędów podczas synchronizacji
    /// </summary>
    public int ErrorsCount { get; set; }
    
    /// <summary>
    /// Komunikat błędu (jeśli wystąpił)
    /// </summary>
    public string ErrorMessage { get; set; }
    
    /// <summary>
    /// Czy synchronizacja została wykonana manualnie (przez endpoint)
    /// </summary>
    public bool IsManual { get; set; }
    
    /// <summary>
    /// Czas trwania synchronizacji w sekundach
    /// </summary>
    public double? DurationSeconds { get; set; }

    public KSeFSynchronizationLogEntity()
    {
        StartedAt = DateTime.UtcNow;
        Status = KSeFSyncStatus.InProgress;
    }

    /// <summary>
    /// Oznacza synchronizację jako zakończoną pomyślnie
    /// </summary>
    public void MarkAsCompleted(int downloaded, int saved)
    {
        CompletedAt = DateTime.UtcNow;
        Status = KSeFSyncStatus.Completed;
        InvoicesDownloaded = downloaded;
        InvoicesSaved = saved;
        DurationSeconds = (CompletedAt.Value - StartedAt).TotalSeconds;
    }

    /// <summary>
    /// Oznacza synchronizację jako zakończoną z błędem
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        CompletedAt = DateTime.UtcNow;
        Status = KSeFSyncStatus.Failed;
        ErrorMessage = errorMessage;
        DurationSeconds = (CompletedAt.Value - StartedAt).TotalSeconds;
    }
}

/// <summary>
/// Status synchronizacji faktur z KSeF
/// </summary>
public enum KSeFSyncStatus
{
    InProgress = 0,
    Completed = 1,
    Failed = 2,
    Cancelled = 3
}
