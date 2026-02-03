using System.Net.Http.Json;
using System.Text.Json;
using FarmsManager.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

public class NbpExchangeRateService : INbpExchangeRateService
{
    private readonly ILogger<NbpExchangeRateService> _logger;
    private readonly HttpClient _httpClient;
    private const string NbpApiBaseUrl = "https://api.nbp.pl/api/exchangerates/rates/a";

    public NbpExchangeRateService(ILogger<NbpExchangeRateService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("NBP");
        _httpClient.BaseAddress = new Uri("https://api.nbp.pl/api/");
    }

    public async Task<decimal> GetExchangeRateAsync(string currencyCode, DateOnly date, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code cannot be empty", nameof(currencyCode));

        if (string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
            return 1.0m;

        try
        {
            var dateString = date.ToString("yyyy-MM-dd");
            var url = $"exchangerates/rates/a/{currencyCode.ToUpper()}/{dateString}/?format=json";

            _logger.LogDebug("Fetching exchange rate for {Currency} on {Date} from NBP API", currencyCode, dateString);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Exchange rate not found for {Currency} on {Date}, trying previous business day", currencyCode, dateString);
                    return await GetExchangeRateForPreviousBusinessDayAsync(currencyCode, date, cancellationToken);
                }

                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadFromJsonAsync<NbpExchangeRateResponse>(cancellationToken);

            if (content?.Rates == null || content.Rates.Count == 0)
            {
                throw new InvalidOperationException($"No exchange rate data returned from NBP API for {currencyCode} on {dateString}");
            }

            var rate = content.Rates[0].Mid;
            _logger.LogInformation("Exchange rate for {Currency} on {Date}: {Rate}", currencyCode, dateString, rate);

            return rate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch exchange rate for {Currency} on {Date}", currencyCode, date);
            throw new InvalidOperationException($"Failed to fetch exchange rate for {currencyCode} on {date}", ex);
        }
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(IEnumerable<string> currencyCodes, DateOnly date, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var currencyCode in currencyCodes.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var rate = await GetExchangeRateAsync(currencyCode, date, cancellationToken);
                result[currencyCode] = rate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch exchange rate for {Currency}", currencyCode);
            }
        }

        return result;
    }

    private async Task<decimal> GetExchangeRateForPreviousBusinessDayAsync(string currencyCode, DateOnly date, CancellationToken cancellationToken, int maxAttempts = 10)
    {
        var currentDate = date.AddDays(-1);
        var attempts = 0;

        while (attempts < maxAttempts)
        {
            if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                currentDate = currentDate.AddDays(-1);
            else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                currentDate = currentDate.AddDays(-2);

            try
            {
                var dateString = currentDate.ToString("yyyy-MM-dd");
                var url = $"exchangerates/rates/a/{currencyCode.ToUpper()}/{dateString}/?format=json";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<NbpExchangeRateResponse>(cancellationToken);

                    if (content?.Rates != null && content.Rates.Count > 0)
                    {
                        var rate = content.Rates[0].Mid;
                        _logger.LogInformation("Found exchange rate for {Currency} on previous business day {Date}: {Rate}", currencyCode, dateString, rate);
                        return rate;
                    }
                }

                currentDate = currentDate.AddDays(-1);
                attempts++;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to fetch rate for {Currency} on {Date}, trying previous day", currencyCode, currentDate);
                currentDate = currentDate.AddDays(-1);
                attempts++;
            }
        }

        throw new InvalidOperationException($"Could not find exchange rate for {currencyCode} within {maxAttempts} business days before {date}");
    }

    private class NbpExchangeRateResponse
    {
        public string Table { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public List<NbpRate> Rates { get; set; }
    }

    private class NbpRate
    {
        public string No { get; set; }
        public string EffectiveDate { get; set; }
        public decimal Mid { get; set; }
    }
}
