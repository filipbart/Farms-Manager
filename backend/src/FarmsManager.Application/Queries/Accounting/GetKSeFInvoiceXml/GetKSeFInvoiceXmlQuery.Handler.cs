using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceXml;

public sealed class GetKSeFInvoiceXmlSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public GetKSeFInvoiceXmlSpec(Guid invoiceId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(x => x.Id == invoiceId);
    }
}

public class GetKSeFInvoiceXmlQueryHandler 
    : IRequestHandler<GetKSeFInvoiceXmlQuery, BaseResponse<KSeFInvoiceXmlDto>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;

    public GetKSeFInvoiceXmlQueryHandler(IKSeFInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<BaseResponse<KSeFInvoiceXmlDto>> Handle(
        GetKSeFInvoiceXmlQuery request, 
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            new GetKSeFInvoiceXmlSpec(request.InvoiceId), cancellationToken);

        if (invoice == null || string.IsNullOrEmpty(invoice.InvoiceXml))
        {
            var errorResponse = new BaseResponse<KSeFInvoiceXmlDto>();
            errorResponse.AddError("NotFound", "Faktura lub XML nie zostały znalezione");
            return errorResponse;
        }

        var filename = $"Faktura_KSeF_{invoice.KSeFNumber ?? invoice.InvoiceNumber}.xml";
        
        return BaseResponse.CreateResponse(new KSeFInvoiceXmlDto
        {
            InvoiceXml = invoice.InvoiceXml,
            FileName = filename
        });
    }
}
