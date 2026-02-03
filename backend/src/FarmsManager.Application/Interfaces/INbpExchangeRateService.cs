using FarmsManager.Application.Common;

namespace FarmsManager.Application.Interfaces;

public interface INbpExchangeRateService : IService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode, DateOnly date, CancellationToken cancellationToken = default);
    
    Task<Dictionary<string, decimal>> GetExchangeRatesAsync(IEnumerable<string> currencyCodes, DateOnly date, CancellationToken cancellationToken = default);
}
