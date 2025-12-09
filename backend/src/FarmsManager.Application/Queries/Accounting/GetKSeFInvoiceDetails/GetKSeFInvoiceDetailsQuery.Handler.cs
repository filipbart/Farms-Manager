using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;

public class GetKSeFInvoiceDetailsQueryHandler 
    : IRequestHandler<GetKSeFInvoiceDetailsQuery, BaseResponse<KSeFInvoiceDetailsDto>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetKSeFInvoiceDetailsQueryHandler(IKSeFInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<KSeFInvoiceDetailsDto>> Handle(
        GetKSeFInvoiceDetailsQuery request, 
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            new GetKSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        if (invoice == null)
        {
            var errorResponse = new BaseResponse<KSeFInvoiceDetailsDto>();
            errorResponse.AddError("NotFound", "Faktura nie została znaleziona");
            return errorResponse;
        }

        var dto = _mapper.Map<KSeFInvoiceDetailsDto>(invoice);

        return BaseResponse.CreateResponse(dto);
    }
}
