using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceXml;

public class KSeFInvoiceXmlDto
{
    public string InvoiceXml { get; set; }
    public string FileName { get; set; }
}

public record GetKSeFInvoiceXmlQuery(Guid InvoiceId) 
    : IRequest<BaseResponse<KSeFInvoiceXmlDto>>;
