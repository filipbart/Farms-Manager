using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;

public record GetKSeFInvoiceDetailsQuery(Guid InvoiceId) 
    : IRequest<BaseResponse<KSeFInvoiceDetailsDto>>;
