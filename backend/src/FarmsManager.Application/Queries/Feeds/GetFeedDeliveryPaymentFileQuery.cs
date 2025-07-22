using Ardalis.Specification;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FluentValidation;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace FarmsManager.Application.Queries.Feeds;

public record GetFeedDeliveryPaymentFileQuery(List<Guid> Ids, string Comment) : IRequest<FileModel>;

public class GetFeedDeliveryPaymentFileQueryValidator : AbstractValidator<GetFeedDeliveryPaymentFileQuery>
{
    public GetFeedDeliveryPaymentFileQueryValidator()
    {
        RuleFor(t => t.Ids).NotEmpty();
    }
}

public class GetFeedDeliveryPaymentFileQueryHandler : IRequestHandler<GetFeedDeliveryPaymentFileQuery, FileModel>
{
    private const string ContentType = "application/pdf";
    private const string Extension = ".pdf";

    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IS3Service _s3Service;

    public GetFeedDeliveryPaymentFileQueryHandler(IFeedInvoiceRepository feedInvoiceRepository, IS3Service s3Service)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
        _s3Service = s3Service;
    }

    public async Task<FileModel> Handle(GetFeedDeliveryPaymentFileQuery request, CancellationToken cancellationToken)
    {
        var invoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesByIdsSpec(request.Ids), cancellationToken);
        var hasDifferentBankAccounts = invoices
            .Select(i => i.BankAccountNumber?.Replace(" ", "").Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .Count() > 1;
        if (hasDifferentBankAccounts)
        {
            throw new Exception("Zaznaczono faktury z różnymi numerami kont bankowych");
        }

        var hasDifferentFarms = invoices
            .Select(i => i.FarmId)
            .Distinct()
            .Count() > 1;
        if (hasDifferentFarms)
        {
            throw new Exception("Zaznaczono faktury z różnych farm");
        }

        if (invoices.Any(t => t.PaymentDateUtc.HasValue))
        {
            throw new Exception("Wybrano fakturę, która jest już opłacona");
        }

        var fileBytes = GeneratePdf(invoices, request.Comment);
        var fileName = $"Przelew_{DateTime.Now:yyyyMMdd_HHmmss}{Extension}";

        await _s3Service.UploadFileAsync(fileBytes, FileType.FeedDeliveryPayment, fileName);

        invoices.ForEach(t => t.MarkAsPaid());
        await _feedInvoiceRepository.UpdateRangeAsync(invoices, cancellationToken);

        return new FileModel
        {
            FileName = fileName,
            IsFile = true,
            CreationDate = DateTime.Now,
            ContentType = ContentType,
            Data = fileBytes
        };
    }

    private static byte[] GeneratePdf(List<FeedInvoiceEntity> invoices, string comment)
    {
        var firstInvoice = invoices.First();
        var totalAmount = invoices.Sum(t => t.InvoiceTotal);
        var totalVat = invoices.Sum(t => t.VatAmount);
        var dueDate = invoices.Min(t => t.DueDate);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(col =>
                {
                    col.Item().PaddingBottom(20).Text(firstInvoice.Farm.Name).FontSize(16).Bold().AlignCenter();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(150);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Numer faktury").Bold();
                            header.Cell().Text("Kwota brutto").Bold();
                            header.Cell().Text("Kwota VAT").Bold();
                        });

                        foreach (var invoice in invoices)
                        {
                            table.Cell().Text(invoice.InvoiceNumber);
                            table.Cell().Text(FormatCurrency(invoice.InvoiceTotal));
                            table.Cell().Text(FormatCurrency(invoice.VatAmount));
                        }
                    });

                    col.Item().PaddingVertical(20).LineHorizontal(1);

                    col.Item().Text($"Suma: {FormatCurrency(totalAmount)}").Bold();
                    col.Item().Text($"Suma VAT: {FormatCurrency(totalVat)}").Bold();
                    col.Item().Text($"Numer rachunku bankowego: {firstInvoice.BankAccountNumber}");
                    col.Item().Text($"Termin płatności: {dueDate:yyyy-MM-dd}");

                    if (!string.IsNullOrWhiteSpace(comment))
                    {
                        col.Item().PaddingTop(20).LineHorizontal(1);
                        col.Item().PaddingTop(10).Text("Komentarz do przelewu:").Bold();
                        col.Item().Text(comment);
                    }
                });
            });
        }).GeneratePdf();

        string FormatCurrency(decimal amount) => $"{amount:N2} zł";
    }
}

public sealed class GetFeedsInvoicesByIdsSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesByIdsSpec(List<Guid> ids)
    {
        EnsureExists();

        Query.Include(t => t.Farm);
        Query.Where(t => ids.Contains(t.Id));
    }
}