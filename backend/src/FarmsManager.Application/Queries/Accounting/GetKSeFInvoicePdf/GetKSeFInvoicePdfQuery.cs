using FarmsManager.Application.FileSystem;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicePdf;

public record GetKSeFInvoicePdfQuery(Guid InvoiceId) : IRequest<FileModel>;
