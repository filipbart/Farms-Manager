using FarmsManager.Application.Common;
using FarmsManager.Application.Models.AzureDi;

namespace FarmsManager.Application.Interfaces;

public interface IAzureDiService : IService
{
    Task<FeedDeliveryInvoiceModel> AnalyzeFeedDeliveryInvoiceAsync(string preSignedUrl);
}