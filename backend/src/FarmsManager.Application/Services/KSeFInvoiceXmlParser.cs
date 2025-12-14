using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Domain.Aggregates.AccountingAggregate.XmlModels;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

/// <summary>
/// Parser XML faktur KSeF
/// </summary>
public class KSeFInvoiceXmlParser : IKSeFInvoiceXmlParser
{
    private readonly ILogger<KSeFInvoiceXmlParser> _logger;
    private readonly IMapper _mapper;

    public KSeFInvoiceXmlParser(ILogger<KSeFInvoiceXmlParser> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Parsuje XML faktury KSeF do modelu
    /// </summary>
    public KSeFInvoiceXml ParseInvoiceXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            _logger.LogWarning("Próba parsowania pustego XML");
            return null;
        }

        try
        {
            // KSeF może zwracać różne wersje schemy, próbujemy z różnymi namespace'ami
            var namespaces = new[]
            {
                "http://crd.gov.pl/wzor/2023/06/29/12648/", // FA(3)
                "http://crd.gov.pl/wzor/2021/11/29/11089/", // FA(2)
                "http://crd.gov.pl/wzor/2021/11/29/11090/" // Alternatywny
            };

            foreach (var ns in namespaces)
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(KSeFInvoiceXml), ns);
                    using var reader = new StringReader(xml);
                    var result = serializer.Deserialize(reader) as KSeFInvoiceXml;
                    if (result != null)
                    {
                        _logger.LogDebug("Pomyślnie sparsowano XML faktury z namespace {Namespace}", ns);
                        return result;
                    }
                }
                catch
                {
                    // Próbuj kolejny namespace
                }
            }

            // Próba bez namespace
            var genericSerializer = new XmlSerializer(typeof(KSeFInvoiceXml));
            using var genericReader = new StringReader(xml);
            return genericSerializer.Deserialize(genericReader) as KSeFInvoiceXml;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas parsowania XML faktury KSeF");
            return null;
        }
    }

    /// <summary>
    /// Konwertuje sparsowany XML na model szczegółów faktury
    /// </summary>
    public KSeFInvoiceDetails ToInvoiceDetails(KSeFInvoiceXml invoice, string ksefNumber)
    {
        if (invoice == null) return null;

        var details = _mapper.Map<KSeFInvoiceDetails>(invoice);
        details.ReferenceNumber = ksefNumber;

        return details;
    }

    /// <summary>
    /// Wyciąga podstawowe dane z XML bez pełnego parsowania
    /// </summary>
    public (decimal GrossAmount, decimal NetAmount, decimal VatAmount, string InvoiceNumber, DateTime? InvoiceDate)
        ExtractBasicData(string xml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("fa", "http://crd.gov.pl/wzor/2023/06/29/12648/");
            nsManager.AddNamespace("fa2", "http://crd.gov.pl/wzor/2021/11/29/11089/");

            // Próba z różnymi namespace'ami
            var grossNode = doc.SelectSingleNode("//fa:P_15", nsManager)
                            ?? doc.SelectSingleNode("//fa2:P_15", nsManager)
                            ?? doc.SelectSingleNode("//*[local-name()='P_15']");

            var invoiceNumberNode = doc.SelectSingleNode("//fa:P_2", nsManager)
                                    ?? doc.SelectSingleNode("//fa2:P_2", nsManager)
                                    ?? doc.SelectSingleNode("//*[local-name()='P_2']");

            var invoiceDateNode = doc.SelectSingleNode("//fa:P_1", nsManager)
                                  ?? doc.SelectSingleNode("//fa2:P_1", nsManager)
                                  ?? doc.SelectSingleNode("//*[local-name()='P_1']");

            var gross = decimal.TryParse(grossNode?.InnerText, out var g) ? g : 0;
            var invoiceNumber = invoiceNumberNode?.InnerText;
            var invoiceDate = DateTime.TryParse(invoiceDateNode?.InnerText, out var d) ? d : (DateTime?)null;

            // Oblicz netto sumując pola P_13_x
            var netAmount = ExtractNetAmount(doc, nsManager);
            var vatAmount = gross - netAmount;

            return (gross, netAmount, vatAmount, invoiceNumber, invoiceDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wyciągania podstawowych danych z XML");
            return (0, 0, 0, null, null);
        }
    }

    private decimal ExtractNetAmount(XmlDocument doc, XmlNamespaceManager nsManager)
    {
        decimal sum = 0;
        var netFields = new[] { "P_13_1", "P_13_2", "P_13_3", "P_13_4", "P_13_5", "P_13_6_1" };

        foreach (var field in netFields)
        {
            var node = doc.SelectSingleNode($"//fa:{field}", nsManager)
                       ?? doc.SelectSingleNode($"//fa2:{field}", nsManager)
                       ?? doc.SelectSingleNode($"//*[local-name()='{field}']");

            if (node != null && decimal.TryParse(node.InnerText, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }
}