using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmsManager.Infrastructure.BackgroundJobs;

/// <summary>
/// BackgroundService wykonujący synchronizację faktur z KSeF
/// </summary>
public class KSeFSynchronizationJob : BackgroundService, IKSeFSynchronizationJob
{
    private readonly ILogger<KSeFSynchronizationJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly KSeFSyncConfiguration _configuration;
    private Timer _timer;

    public KSeFSynchronizationJob(
        ILogger<KSeFSynchronizationJob> logger,
        IServiceProvider serviceProvider,
        IOptions<KSeFSyncConfiguration> configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Sprawdzenie czy synchronizacja jest włączona
        if (!_configuration.Enabled)
        {
            _logger.LogInformation("KSeF synchronization job is disabled in configuration");
            return;
        }

        _logger.LogInformation(
            "KSeF synchronization job started. Interval: {IntervalHours} hours",
            _configuration.IntervalHours);

        // Obliczenie interwału w milisekundach
        var intervalMilliseconds = TimeSpan.FromHours(_configuration.IntervalHours).TotalMilliseconds;

        // Utworzenie timera wykonującego się cyklicznie
        _timer = new Timer(
            async void (_) =>
            {
                try
                {
                    await ExecuteSynchronizationAsync(isManual: false, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "KSeF synchronization job failed");
                }
            },
            null,
            TimeSpan.Zero, // Pierwsze wykonanie od razu po starcie
            TimeSpan.FromMilliseconds(intervalMilliseconds));

        // Utrzymanie serwisu przy życiu
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// Wykonuje synchronizację faktur z KSeF
    /// </summary>
    public async Task ExecuteSynchronizationAsync(bool isManual = false, CancellationToken cancellationToken = default)
    {
        // Utworzenie scope dla scoped services
        using var scope = _serviceProvider.CreateScope();

        var syncLogRepository = scope.ServiceProvider.GetRequiredService<IKSeFSynchronizationLogRepository>();
        var ksefService = scope.ServiceProvider.GetRequiredService<IKSeFService>();

        var log = new KSeFSynchronizationLogEntity
        {
            IsManual = isManual
        };

        try
        {
            _logger.LogInformation(
                "Starting KSeF synchronization. Manual: {IsManual}",
                isManual);

            // ========================================
            // TODO: TUTAJ DODASZ SWOJĄ LOGIKĘ
            // ========================================

            // Przykład:
            // 1. Pobierz faktury z KSeF używając ksefService.GetInvoicesAsync()
            // 2. Zapisz faktury do bazy danych
            // 3. Zlicz pobrane i zapisane faktury

            // PLACEHOLDER - symulacja pobierania faktur
            await Task.Delay(1000, cancellationToken);

            // PRZYKŁADOWE wartości - zastąp swoją logiką
            int downloadedCount = 0; // Liczba pobranych faktur
            int savedCount = 0; // Liczba zapisanych faktur

            // ========================================
            // KONIEC TODO
            // ========================================

            log.MarkAsCompleted(downloadedCount, savedCount);

            _logger.LogInformation(
                "KSeF synchronization completed successfully. Downloaded: {Downloaded}, Saved: {Saved}, Duration: {Duration}s",
                downloadedCount,
                savedCount,
                log.DurationSeconds);
        }
        catch (Exception ex)
        {
            log.ErrorsCount++;
            log.MarkAsFailed(ex.Message);

            _logger.LogError(ex,
                "KSeF synchronization failed. Error: {ErrorMessage}",
                ex.Message);
        }
        finally
        {
            // Zapisz log do bazy
            await syncLogRepository.AddAsync(log, cancellationToken);
            await syncLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}