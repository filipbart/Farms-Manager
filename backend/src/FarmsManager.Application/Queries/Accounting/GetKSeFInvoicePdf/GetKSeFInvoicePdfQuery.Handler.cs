using System.Globalization;
using System.Xml.Linq;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicePdf;

public class GetKSeFInvoicePdfQueryHandler : IRequestHandler<GetKSeFInvoicePdfQuery, FileModel>
{
    private const string ContentType = "application/pdf";
    private const string Extension = ".pdf";

    private readonly IKSeFInvoiceRepository _invoiceRepository;

    public GetKSeFInvoicePdfQueryHandler(IKSeFInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<FileModel> Handle(GetKSeFInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            new GetKSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        if (invoice == null)
        {
            throw DomainException.RecordNotFound("Faktura nie została znaleziona");
        }

        var parsedData = !string.IsNullOrEmpty(invoice.InvoiceXml) 
            ? ParseInvoiceXml(invoice.InvoiceXml) 
            : null;

        var fileBytes = GenerateInvoicePdf(invoice, parsedData);
        var fileName = $"Faktura_{SanitizeFileName(invoice.InvoiceNumber)}_{invoice.InvoiceDate:yyyy-MM-dd}{Extension}";

        return new FileModel
        {
            FileName = fileName,
            IsFile = true,
            CreationDate = DateTime.Now,
            ContentType = ContentType,
            Data = fileBytes
        };
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }

    private static byte[] GenerateInvoicePdf(KSeFInvoiceEntity invoice, ParsedInvoiceData parsedData)
    {
        var primaryColor = Color.FromHex("#0D1B2A");
        var culture = new CultureInfo("pl-PL");

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(c => ComposeHeader(c, invoice, parsedData, primaryColor));
                page.Content().Element(c => ComposeContent(c, invoice, parsedData, primaryColor, culture));
                page.Footer().Element(c => ComposeFooter(c, invoice));
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, KSeFInvoiceEntity invoice, 
        ParsedInvoiceData parsedData, Color primaryColor)
    {
        container.Column(col =>
        {
            // Tytuł faktury
            col.Item().PaddingBottom(5).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Faktura").FontSize(24).Bold().FontColor(primaryColor);
                    c.Item().Background(Colors.Grey.Lighten3).Padding(5).Text(text =>
                    {
                        text.Span("Nr ").FontSize(10);
                        text.Span(invoice.InvoiceNumber).FontSize(10).Bold();
                    });
                    if (!string.IsNullOrEmpty(invoice.KSeFNumber))
                    {
                        c.Item().PaddingTop(3).Text(text =>
                        {
                            text.Span("KSeF: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            text.Span(invoice.KSeFNumber).FontSize(8).FontColor(Colors.Grey.Darken1);
                        });
                    }
                });
            });

            col.Item().PaddingTop(15).PaddingBottom(10).BorderBottom(2).BorderColor(primaryColor);

            // Sprzedawca i Nabywca
            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("SPRZEDAWCA:").FontSize(8).FontColor(Colors.Grey.Darken1);
                    c.Item().PaddingTop(3).Text(parsedData?.SellerName ?? invoice.SellerName ?? "—")
                        .FontSize(10).Bold();
                    if (!string.IsNullOrEmpty(parsedData?.SellerAddress))
                    {
                        c.Item().Text(parsedData.SellerAddress).FontSize(9);
                    }
                    c.Item().PaddingTop(3).Text(text =>
                    {
                        text.Span("NIP: ").Bold().FontSize(9);
                        text.Span(invoice.SellerNip ?? "—").FontSize(9);
                    });
                    if (!string.IsNullOrEmpty(parsedData?.SellerPhone))
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("Tel: ").Bold().FontSize(9);
                            text.Span(parsedData.SellerPhone).FontSize(9);
                        });
                    }
                });

                row.ConstantItem(30);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("NABYWCA:").FontSize(8).FontColor(Colors.Grey.Darken1);
                    c.Item().PaddingTop(3).Text(parsedData?.BuyerName ?? invoice.BuyerName ?? "—")
                        .FontSize(10).Bold();
                    if (!string.IsNullOrEmpty(parsedData?.BuyerAddress))
                    {
                        c.Item().Text(parsedData.BuyerAddress).FontSize(9);
                    }
                    c.Item().PaddingTop(3).Text(text =>
                    {
                        text.Span("NIP: ").Bold().FontSize(9);
                        text.Span(invoice.BuyerNip ?? "—").FontSize(9);
                    });
                    if (!string.IsNullOrEmpty(parsedData?.BuyerPhone))
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("Tel: ").Bold().FontSize(9);
                            text.Span(parsedData.BuyerPhone).FontSize(9);
                        });
                    }
                });
            });

            col.Item().PaddingTop(10).PaddingBottom(10).BorderBottom(2).BorderColor(primaryColor);

            // Daty i płatność
            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("DATA WYSTAWIENIA: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                        text.Span(invoice.InvoiceDate.ToString("yyyy-MM-dd")).FontSize(9);
                    });
                    if (parsedData?.SaleDate != null)
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("DATA SPRZEDAŻY: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            text.Span(parsedData.SaleDate.Value.ToString("yyyy-MM-dd")).FontSize(9);
                        });
                    }
                    if (parsedData?.DueDate != null)
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("TERMIN PŁATNOŚCI: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            text.Span(parsedData.DueDate.Value.ToString("yyyy-MM-dd")).FontSize(9);
                        });
                    }
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("SPOSÓB PŁATNOŚCI: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                        text.Span(parsedData?.PaymentMethod ?? GetPaymentTypeLabel(invoice.PaymentType)).FontSize(9);
                    });
                    if (!string.IsNullOrEmpty(parsedData?.BankAccount))
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("NUMER KONTA: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            text.Span(parsedData.BankAccount).FontSize(9);
                        });
                    }
                    if (!string.IsNullOrEmpty(parsedData?.BankName))
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("BANK: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            text.Span(parsedData.BankName).FontSize(9);
                        });
                    }
                });
            });

            col.Item().PaddingTop(10).PaddingBottom(5).BorderBottom(2).BorderColor(primaryColor);
        });
    }

    private static void ComposeContent(IContainer container, KSeFInvoiceEntity invoice, 
        ParsedInvoiceData parsedData, Color primaryColor, CultureInfo culture)
    {
        container.PaddingTop(10).Column(col =>
        {
            // Tabela pozycji
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);  // Lp.
                    columns.RelativeColumn(3);   // Nazwa
                    columns.ConstantColumn(45);  // Ilość
                    columns.ConstantColumn(35);  // Jm
                    columns.ConstantColumn(65);  // Cena netto
                    columns.ConstantColumn(70);  // Wartość netto
                    columns.ConstantColumn(45);  // Stawka VAT
                    columns.ConstantColumn(60);  // Kwota VAT
                    columns.ConstantColumn(70);  // Wartość brutto
                });

                // Nagłówek tabeli
                table.Header(header =>
                {
                    header.Cell().Background(primaryColor).Padding(4)
                        .Text("Lp.").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4)
                        .Text("Nazwa").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignCenter()
                        .Text("Ilość").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignCenter()
                        .Text("Jm").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignRight()
                        .Text("Cena netto").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignRight()
                        .Text("Wart. netto").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignCenter()
                        .Text("VAT").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignRight()
                        .Text("Kwota VAT").FontColor(Colors.White).FontSize(8).Bold();
                    header.Cell().Background(primaryColor).Padding(4).AlignRight()
                        .Text("Wart. brutto").FontColor(Colors.White).FontSize(8).Bold();
                });

                // Pozycje faktury
                if (parsedData?.LineItems != null && parsedData.LineItems.Count > 0)
                {
                    var lineNumber = 1;
                    foreach (var item in parsedData.LineItems)
                    {
                        var vatAmount = item.GrossAmount.HasValue && item.NetAmount.HasValue
                            ? item.GrossAmount.Value - item.NetAmount.Value
                            : (item.NetAmount.HasValue && item.VatRate.HasValue
                                ? item.NetAmount.Value * (item.VatRate.Value / 100m)
                                : 0);

                        var grossAmount = item.GrossAmount ?? 
                            (item.NetAmount.HasValue ? item.NetAmount.Value + vatAmount : 0);

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3)
                            .Text(lineNumber.ToString()).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3)
                            .Text(item.Name ?? "—").FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter()
                            .Text(item.Quantity?.ToString("N2", culture) ?? "—").FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter()
                            .Text(item.Unit ?? "—").FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight()
                            .Text(FormatCurrency(item.UnitPriceNet, culture)).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight()
                            .Text(FormatCurrency(item.NetAmount, culture)).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter()
                            .Text(item.VatRate.HasValue ? $"{item.VatRate.Value}%" : "zw.").FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight()
                            .Text(FormatCurrency(vatAmount, culture)).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight()
                            .Text(FormatCurrency(grossAmount, culture)).FontSize(8);

                        lineNumber++;
                    }
                }
                else
                {
                    // Jeśli brak sparsowanych pozycji, wyświetl ogólne informacje
                    table.Cell().ColumnSpan(9).Padding(10).AlignCenter()
                        .Text("Szczegółowe pozycje faktury niedostępne").FontColor(Colors.Grey.Darken1);
                }
            });

            col.Item().PaddingTop(15);

            // Podsumowanie
            col.Item().Row(row =>
            {
                // Razem do zapłaty - box
                row.RelativeItem().Background(primaryColor).Padding(10).Column(c =>
                {
                    c.Item().Text("RAZEM DO ZAPŁATY:").FontColor(Colors.White).FontSize(10);
                    c.Item().PaddingTop(3).Text(FormatCurrency(invoice.GrossAmount, culture))
                        .FontColor(Colors.White).FontSize(16).Bold();
                });

                row.ConstantItem(20);

                // Tabela podsumowania VAT
                row.RelativeItem(2).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(80);
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(70);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("").FontSize(7);
                        header.Cell().AlignRight().Text("Wartość netto").FontSize(7);
                        header.Cell().AlignCenter().Text("Stawka").FontSize(7);
                        header.Cell().AlignRight().Text("Kwota VAT").FontSize(7);
                        header.Cell().AlignRight().Text("Wartość brutto").FontSize(7);
                    });

                    table.Cell().Text("Razem:").FontColor(primaryColor).Bold().FontSize(9);
                    table.Cell().AlignRight().Text(FormatCurrency(invoice.NetAmount, culture))
                        .FontColor(primaryColor).FontSize(9);
                    table.Cell().AlignCenter().Text("X").FontColor(primaryColor).FontSize(9);
                    table.Cell().AlignRight().Text(FormatCurrency(invoice.VatAmount, culture))
                        .FontColor(primaryColor).FontSize(9);
                    table.Cell().AlignRight().Text(FormatCurrency(invoice.GrossAmount, culture))
                        .FontColor(primaryColor).FontSize(9);

                    // Rozbicie VAT jeśli dostępne
                    if (parsedData?.VatBreakdown != null)
                    {
                        foreach (var vat in parsedData.VatBreakdown)
                        {
                            table.Cell().Text("W tym:").FontSize(8);
                            table.Cell().AlignRight().Text(FormatCurrency(vat.NetAmount, culture)).FontSize(8);
                            table.Cell().AlignCenter().Text(vat.Rate ?? "zw.").FontSize(8);
                            table.Cell().AlignRight().Text(FormatCurrency(vat.VatAmount, culture)).FontSize(8);
                            table.Cell().AlignRight().Text(FormatCurrency(vat.NetAmount + vat.VatAmount, culture)).FontSize(8);
                        }
                    }
                });
            });

            col.Item().PaddingTop(15);

            // Status płatności
            col.Item().Row(row =>
            {
                var paidAmount = CalculatePaidAmount(invoice.PaymentStatus, invoice.GrossAmount);
                var remainingAmount = invoice.GrossAmount - paidAmount;

                row.RelativeItem().Text(text =>
                {
                    text.Span("Zapłacono: ").Bold().FontSize(9);
                    text.Span(FormatCurrency(paidAmount, culture)).FontSize(9);
                });
                row.RelativeItem().Text(text =>
                {
                    text.Span("Pozostało do zapłaty: ").Bold().FontSize(9);
                    text.Span(FormatCurrency(remainingAmount, culture)).FontSize(9)
                        .FontColor(remainingAmount > 0 ? Colors.Red.Darken1 : Colors.Green.Darken1);
                });
            });

            col.Item().PaddingTop(5);

            // Kwota słownie
            col.Item().Text(text =>
            {
                text.Span("Słownie: ").Bold().FontSize(9);
                text.Span(NumberToWords(invoice.GrossAmount)).FontSize(9).Italic();
            });

            // Uwagi/komentarz
            if (!string.IsNullOrEmpty(invoice.Comment) || !string.IsNullOrEmpty(parsedData?.Footer))
            {
                col.Item().PaddingTop(15).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10);
                col.Item().Text("Uwagi:").Bold().FontSize(9);
                if (!string.IsNullOrEmpty(parsedData?.Footer))
                {
                    col.Item().Text(parsedData.Footer).FontSize(8);
                }
                if (!string.IsNullOrEmpty(invoice.Comment))
                {
                    col.Item().PaddingTop(3).Text(invoice.Comment).FontSize(8);
                }
            }
        });
    }

    private static void ComposeFooter(IContainer container, KSeFInvoiceEntity invoice)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignLeft().Text(text =>
            {
                text.Span("Źródło: ").FontSize(7).FontColor(Colors.Grey.Darken1);
                text.Span(GetSourceLabel(invoice.InvoiceSource)).FontSize(7).FontColor(Colors.Grey.Darken1);
            });
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Wygenerowano: ").FontSize(7).FontColor(Colors.Grey.Darken1);
                text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontSize(7).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private static string FormatCurrency(decimal? amount, CultureInfo culture)
    {
        if (!amount.HasValue) return "—";
        return $"{amount.Value.ToString("N2", culture)} zł";
    }

    private static string GetPaymentTypeLabel(KSeFInvoicePaymentType paymentType)
    {
        return paymentType switch
        {
            KSeFInvoicePaymentType.Cash => "Gotówka",
            KSeFInvoicePaymentType.BankTransfer => "Przelew",
            _ => "—"
        };
    }

    private static string GetSourceLabel(KSeFInvoiceSource source)
    {
        return source switch
        {
            KSeFInvoiceSource.KSeF => "KSeF",
            KSeFInvoiceSource.Manual => "Poza KSeF",
            _ => "—"
        };
    }

    private static decimal CalculatePaidAmount(KSeFPaymentStatus paymentStatus, decimal grossAmount)
    {
        return paymentStatus switch
        {
            KSeFPaymentStatus.PaidCash or KSeFPaymentStatus.PaidTransfer => grossAmount,
            KSeFPaymentStatus.PartiallyPaid => grossAmount * 0.5m,
            _ => 0m
        };
    }

    private static string MapPaymentMethodCode(string code)
    {
        return code switch
        {
            "1" => "Gotówka",
            "2" => "Karta",
            "3" => "Bon",
            "4" => "Czek",
            "5" => "Kredyt",
            "6" => "Przelew",
            "7" => "Mobilna",
            _ => code ?? "—"
        };
    }

    private static string NumberToWords(decimal num)
    {
        if (num == 0) return "zero PLN";

        var ones = new[] { "", "jeden", "dwa", "trzy", "cztery", "pięć", "sześć", "siedem", "osiem", "dziewięć" };
        var teens = new[] { "dziesięć", "jedenaście", "dwanaście", "trzynaście", "czternaście", "piętnaście", "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście" };
        var tens = new[] { "", "", "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt", "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt" };
        var hundreds = new[] { "", "sto", "dwieście", "trzysta", "czterysta", "pięćset", "sześćset", "siedemset", "osiemset", "dziewięćset" };

        string ConvertGroup(int n)
        {
            if (n == 0) return "";
            if (n < 10) return ones[n];
            if (n < 20) return teens[n - 10];
            if (n < 100) return tens[n / 10] + (n % 10 != 0 ? " " + ones[n % 10] : "");
            return hundreds[n / 100] + (n % 100 != 0 ? " " + ConvertGroup(n % 100) : "");
        }

        var intPart = (int)Math.Floor(num);
        var decPart = (int)Math.Round((num - intPart) * 100);
        var result = "";

        if (intPart >= 1000000)
        {
            var millions = intPart / 1000000;
            if (millions == 1) result += "milion ";
            else if (millions >= 2 && millions <= 4) result += ConvertGroup(millions) + " miliony ";
            else result += ConvertGroup(millions) + " milionów ";
        }

        if (intPart >= 1000)
        {
            var thousands = (intPart % 1000000) / 1000;
            if (thousands > 0)
            {
                if (thousands == 1) result += "tysiąc ";
                else if (thousands >= 2 && thousands <= 4) result += ConvertGroup(thousands) + " tysiące ";
                else if (thousands >= 5 && thousands <= 21) result += ConvertGroup(thousands) + " tysięcy ";
                else
                {
                    var lastDigit = thousands % 10;
                    if (lastDigit >= 2 && lastDigit <= 4) result += ConvertGroup(thousands) + " tysiące ";
                    else result += ConvertGroup(thousands) + " tysięcy ";
                }
            }
        }

        var remainder = intPart % 1000;
        if (remainder > 0) result += ConvertGroup(remainder);

        result = result.Trim() + " PLN";
        result += " " + decPart.ToString("00") + "/100";

        return result;
    }

    private static ParsedInvoiceData ParseInvoiceXml(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

            var data = new ParsedInvoiceData();

            // Sprzedawca
            var seller = doc.Descendants(ns + "Podmiot1").FirstOrDefault();
            if (seller != null)
            {
                data.SellerName = seller.Descendants(ns + "Nazwa").FirstOrDefault()?.Value;
                data.SellerNip = seller.Descendants(ns + "NIP").FirstOrDefault()?.Value;
                var sellerAddr = seller.Descendants(ns + "Adres").FirstOrDefault();
                if (sellerAddr != null)
                {
                    var street = sellerAddr.Element(ns + "AdresL1")?.Value;
                    var city = sellerAddr.Element(ns + "AdresL2")?.Value;
                    data.SellerAddress = string.Join(", ", new[] { street, city }.Where(x => !string.IsNullOrEmpty(x)));
                }
                data.SellerPhone = seller.Descendants(ns + "Telefon").FirstOrDefault()?.Value;
            }

            // Nabywca
            var buyer = doc.Descendants(ns + "Podmiot2").FirstOrDefault();
            if (buyer != null)
            {
                data.BuyerName = buyer.Descendants(ns + "Nazwa").FirstOrDefault()?.Value;
                data.BuyerNip = buyer.Descendants(ns + "NIP").FirstOrDefault()?.Value;
                var buyerAddr = buyer.Descendants(ns + "Adres").FirstOrDefault();
                if (buyerAddr != null)
                {
                    var street = buyerAddr.Element(ns + "AdresL1")?.Value;
                    var city = buyerAddr.Element(ns + "AdresL2")?.Value;
                    data.BuyerAddress = string.Join(", ", new[] { street, city }.Where(x => !string.IsNullOrEmpty(x)));
                }
                data.BuyerPhone = buyer.Descendants(ns + "Telefon").FirstOrDefault()?.Value;
            }

            // Daty
            var invoiceHeader = doc.Descendants(ns + "Fa").FirstOrDefault();
            if (invoiceHeader != null)
            {
                var saleDate = invoiceHeader.Element(ns + "P_6")?.Value;
                if (DateOnly.TryParse(saleDate, out var sd)) data.SaleDate = sd;

                // Płatność
                var payment = invoiceHeader.Element(ns + "Platnosc");
                if (payment != null)
                {
                    var paymentCode = payment.Element(ns + "FormaPlatnosci")?.Value;
                    data.PaymentMethod = MapPaymentMethodCode(paymentCode);
                    var dueDate = payment.Element(ns + "TerminPlatnosci")?.Element(ns + "Termin")?.Value;
                    if (DateOnly.TryParse(dueDate, out var dd)) data.DueDate = dd;

                    var bankAccount = payment.Descendants(ns + "NrRB").FirstOrDefault()?.Value;
                    data.BankAccount = bankAccount;
                    data.BankName = payment.Descendants(ns + "NazwaBanku").FirstOrDefault()?.Value;
                }
            }

            // Pozycje faktury
            var lineItems = doc.Descendants(ns + "FaWiersz");
            data.LineItems = new List<InvoiceLineItem>();
            foreach (var item in lineItems)
            {
                var lineItem = new InvoiceLineItem
                {
                    Name = item.Element(ns + "P_7")?.Value,
                    Unit = item.Element(ns + "P_8A")?.Value,
                };

                if (decimal.TryParse(item.Element(ns + "P_8B")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty))
                    lineItem.Quantity = qty;
                if (decimal.TryParse(item.Element(ns + "P_9A")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var unitPrice))
                    lineItem.UnitPriceNet = unitPrice;
                if (decimal.TryParse(item.Element(ns + "P_11")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var netAmount))
                    lineItem.NetAmount = netAmount;
                if (decimal.TryParse(item.Element(ns + "P_11A")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var grossAmount))
                    lineItem.GrossAmount = grossAmount;
                if (decimal.TryParse(item.Element(ns + "P_12")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var vatRate))
                    lineItem.VatRate = vatRate;

                data.LineItems.Add(lineItem);
            }

            // Rozbicie VAT
            var vatSummary = doc.Descendants(ns + "P_13_1");
            data.VatBreakdown = new List<VatBreakdownItem>();
            foreach (var vatItem in vatSummary)
            {
                var vbi = new VatBreakdownItem();
                if (decimal.TryParse(vatItem.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var vatNet))
                    vbi.NetAmount = vatNet;
                data.VatBreakdown.Add(vbi);
            }

            // Stopka/uwagi
            data.Footer = doc.Descendants(ns + "DodatkowyOpis").FirstOrDefault()?.Value;

            return data;
        }
        catch
        {
            return null;
        }
    }

    private class ParsedInvoiceData
    {
        public string SellerName { get; set; }
        public string SellerNip { get; set; }
        public string SellerAddress { get; set; }
        public string SellerPhone { get; set; }
        public string BuyerName { get; set; }
        public string BuyerNip { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerPhone { get; set; }
        public DateOnly? SaleDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public List<InvoiceLineItem> LineItems { get; set; }
        public List<VatBreakdownItem> VatBreakdown { get; set; }
        public string Footer { get; set; }
    }

    private class InvoiceLineItem
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPriceNet { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? VatRate { get; set; }
    }

    private class VatBreakdownItem
    {
        public string Rate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal VatAmount { get; set; }
    }
}
