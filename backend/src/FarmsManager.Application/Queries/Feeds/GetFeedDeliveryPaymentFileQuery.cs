using Ardalis.Specification;
using FarmsManager.Application.Commands.Feeds.Deliveries;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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

    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedPaymentRepository _feedPaymentRepository;
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;

    public GetFeedDeliveryPaymentFileQueryHandler(IS3Service s3Service, IUserDataResolver userDataResolver,
        IFeedInvoiceRepository feedInvoiceRepository, IFeedPaymentRepository feedPaymentRepository,
        IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _s3Service = s3Service;
        _userDataResolver = userDataResolver;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedPaymentRepository = feedPaymentRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    public async Task<FileModel> Handle(GetFeedDeliveryPaymentFileQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var invoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesByIdsSpec(request.Ids), cancellationToken);

        var corrections =
            await _feedInvoiceCorrectionRepository.ListAsync(new GetFeedsCorrectionsByIdsSpec(request.Ids),
                cancellationToken);

        corrections = corrections.DistinctBy(t => t.Id).ToList();

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

        var firstInvoice = invoices.First();
        if (corrections.Any(t => t.FarmId != firstInvoice.FarmId))
        {
            throw new Exception("Zaznaczono faktury z różnych farm");
        }

        if (invoices.Any(t => t.PaymentDateUtc.HasValue))
        {
            throw new Exception("Wybrano fakturę, która jest już opłacona");
        }

        var fileBytes = GeneratePdf(invoices, corrections, request.Comment);
        var fileName = $"Przelew_{DateTime.Now:yyyyMMdd_HHmmss}{Extension}";

        var filePath =
            await _s3Service.UploadFileAsync(fileBytes, FileType.FeedDeliveryPayment, fileName, cancellationToken);

        var invoice = invoices.First();
        var newFeedPayment = FeedPaymentEntity.CreateNew(invoice.FarmId, invoice.CycleId, fileName, filePath, userId);
        await _feedPaymentRepository.AddAsync(newFeedPayment, cancellationToken);

        invoices.ForEach(t => t.MarkAsPaid(newFeedPayment.Id));
        await _feedInvoiceRepository.UpdateRangeAsync(invoices, cancellationToken);

        corrections.ForEach(t => t.MarkAsPaid(newFeedPayment.Id));
        await _feedInvoiceCorrectionRepository.UpdateRangeAsync(corrections, cancellationToken);


        return new FileModel
        {
            FileName = fileName,
            IsFile = true,
            CreationDate = DateTime.Now,
            ContentType = ContentType,
            Data = fileBytes
        };
    }

    private static byte[] GeneratePdf(
        List<FeedInvoiceEntity> invoices,
        List<FeedInvoiceCorrectionEntity> corrections,
        string comment)
    {
        var firstInvoice = invoices.First();

        var totalAmount = invoices.Sum(i => i.InvoiceTotal);
        var totalSubTotal = invoices.Sum(i => i.SubTotal);
        var totalVat = invoices.Sum(i => i.VatAmount);

        var correctionAmount = corrections.Sum(c => c.InvoiceTotal);
        var correctionSubTotal = corrections.Sum(c => c.SubTotal);
        var correctionVat = corrections.Sum(c => c.VatAmount);

        var finalAmount = totalAmount + correctionAmount;
        var finalSubTotal = totalSubTotal + correctionSubTotal;
        var finalVat = totalVat + correctionVat;

        var dueDate = invoices.Min(i => i.DueDate);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(col =>
                {
                    col.Item().PaddingBottom(20)
                        .Text(firstInvoice.Farm.Name)
                        .FontSize(16).Bold().AlignCenter();

                    // Sekcja: Faktury
                    col.Item().PaddingBottom(10).Text("Faktury").Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(150); // Numer
                            columns.RelativeColumn(); // Brutto
                            columns.RelativeColumn(); // Netto
                            columns.RelativeColumn(); // VAT
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Numer faktury").Bold();
                            header.Cell().Text("Kwota brutto").Bold();
                            header.Cell().Text("Kwota netto").Bold();
                            header.Cell().Text("Kwota VAT").Bold();
                        });

                        foreach (var invoice in invoices)
                        {
                            table.Cell().Text(invoice.InvoiceNumber);
                            table.Cell().Text(FormatCurrency(invoice.InvoiceTotal));
                            table.Cell().Text(FormatCurrency(invoice.SubTotal));
                            table.Cell().Text(FormatCurrency(invoice.VatAmount));
                        }

                        table.Cell().ColumnSpan(4).PaddingVertical(5).LineHorizontal(1);

                        table.Cell().Text("Suma brutto:").Bold();
                        table.Cell().Text(FormatCurrency(totalAmount)).Bold();
                        table.Cell().Text(string.Empty);
                        table.Cell().Text(string.Empty);

                        table.Cell().Text("Suma netto:").Bold();
                        table.Cell().Text(FormatCurrency(totalSubTotal)).Bold();
                        table.Cell().Text(string.Empty);
                        table.Cell().Text(string.Empty);

                        table.Cell().Text("Suma VAT:").Bold();
                        table.Cell().Text(FormatCurrency(totalVat)).Bold();
                        table.Cell().Text(string.Empty);
                        table.Cell().Text(string.Empty);
                    });

                    // Sekcja: Korekty (jeśli są)
                    if (corrections.Count != 0)
                    {
                        col.Item().PaddingBottom(10).PaddingTop(20).Text("Faktury korygujące").Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150); // Numer
                                columns.RelativeColumn(); // Brutto
                                columns.RelativeColumn(); // Netto
                                columns.RelativeColumn(); // VAT
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Numer korekty").Bold();
                                header.Cell().Text("Kwota brutto").Bold();
                                header.Cell().Text("Kwota netto").Bold();
                                header.Cell().Text("Kwota VAT").Bold();
                            });

                            foreach (var correction in corrections)
                            {
                                table.Cell().Text(correction.InvoiceNumber);
                                table.Cell().Text(FormatCurrency(correction.InvoiceTotal));
                                table.Cell().Text(FormatCurrency(correction.SubTotal));
                                table.Cell().Text(FormatCurrency(correction.VatAmount));
                            }

                            table.Cell().ColumnSpan(4).PaddingVertical(5).LineHorizontal(1);

                            table.Cell().Text("Suma brutto (korekty):").Bold();
                            table.Cell().Text(FormatCurrency(correctionAmount)).Bold();
                            table.Cell().Text(string.Empty);
                            table.Cell().Text(string.Empty);

                            table.Cell().Text("Suma netto (korekty):").Bold();
                            table.Cell().Text(FormatCurrency(correctionSubTotal)).Bold();
                            table.Cell().Text(string.Empty);
                            table.Cell().Text(string.Empty);

                            table.Cell().Text("Suma VAT (korekty):").Bold();
                            table.Cell().Text(FormatCurrency(correctionVat)).Bold();
                            table.Cell().Text(string.Empty);
                            table.Cell().Text(string.Empty);
                        });

                        // Sekcja: Po korekcie
                        col.Item().PaddingTop(20).Text("Po korekcie").Bold();
                        AddSummaryTable(col, finalAmount, finalSubTotal, finalVat);
                    }

                    // Info bankowe
                    col.Item().PaddingVertical(20).LineHorizontal(1);
                    col.Item().Text($"Numer rachunku bankowego: {firstInvoice.BankAccountNumber}");
                    col.Item().Text($"Termin płatności: {dueDate:yyyy-MM-dd}");

                    // Komentarz
                    if (!string.IsNullOrWhiteSpace(comment))
                    {
                        col.Item().PaddingTop(20).LineHorizontal(1);
                        col.Item().PaddingTop(10).Text("Komentarz do przelewu:").Bold();
                        col.Item().Text(comment);
                    }
                });
            });
        }).GeneratePdf();

        void AddSummaryTable(ColumnDescriptor col, decimal amount, decimal subTotal, decimal vat)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(200);
                    columns.RelativeColumn();
                });

                table.Cell().Text("Suma brutto:");
                table.Cell().Text(FormatCurrency(amount));

                table.Cell().Text("Suma netto:");
                table.Cell().Text(FormatCurrency(subTotal));

                table.Cell().Text("Suma VAT:");
                table.Cell().Text(FormatCurrency(vat));
            });
        }

        string FormatCurrency(decimal amount) => $"{amount:N2} zł";
    }
}

public sealed class GetFeedsInvoicesByIdsSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesByIdsSpec(List<Guid> ids)
    {
        EnsureExists();

        Query.Include(t => t.Farm);
        Query.Include(t => t.InvoiceCorrection);
        Query.Where(t => ids.Contains(t.Id));
        Query.OrderBy(t => t.InvoiceNumber);
    }
}