using FarmsManager.Application.Common;

namespace FarmsManager.Application.Interfaces;

public interface IAzureDiService : IService
{
    Task<T> AnalyzeInvoiceAsync<T>(string preSignedUrl) where T : class, new();
}