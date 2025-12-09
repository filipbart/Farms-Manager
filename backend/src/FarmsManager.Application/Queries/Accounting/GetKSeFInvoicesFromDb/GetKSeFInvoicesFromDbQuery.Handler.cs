using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;

public class GetKSeFInvoicesFromDbQueryHandler 
    : IRequestHandler<GetKSeFInvoicesFromDbQuery, BaseResponse<GetKSeFInvoicesFromDbQueryResponse>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetKSeFInvoicesFromDbQueryHandler(IKSeFInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetKSeFInvoicesFromDbQueryResponse>> Handle(
        GetKSeFInvoicesFromDbQuery request, 
        CancellationToken cancellationToken)
    {
        var filters = request.Filters;
        
        var data = await _invoiceRepository.ListAsync(
            new GetKSeFInvoicesFromDbSpec(filters, true), cancellationToken);

        var count = await _invoiceRepository.CountAsync(
            new GetKSeFInvoicesFromDbSpec(filters, false), cancellationToken);

        var items = _mapper.Map<List<KSeFInvoiceFromDbDto>>(data);

        return BaseResponse.CreateResponse(new GetKSeFInvoicesFromDbQueryResponse
        {
            Items = items,
            TotalRows = count
        });
    }
}
