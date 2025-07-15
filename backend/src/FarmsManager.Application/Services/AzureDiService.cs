using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;

namespace FarmsManager.Application.Services;

public class AzureDiService : IAzureDiService
{
    public Task<FeedDeliveryInvoiceModel> AnalyzeFeedDeliveryInvoiceAsync(string preSignedUrl)
    {
        throw new NotImplementedException();
    }
}