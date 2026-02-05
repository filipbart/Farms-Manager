using System.Text;
using System.Text.RegularExpressions;
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
public partial class KSeFInvoiceXmlParser : IKSeFInvoiceXmlParser
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
                "http://crd.gov.pl/wzor/2025/06/25/13775/", // FA(4) - najnowszy
                "http://crd.gov.pl/wzor/2023/06/29/12648/", // FA(3)
                "http://crd.gov.pl/wzor/2021/11/29/11089/", // FA(2)
                "http://crd.gov.pl/wzor/2021/11/29/11090/" // Alternatywny
            };

            // Najpierw spróbuj z oryginalnym XML (dla kompatybilności wstecznej)
            foreach (var ns in namespaces)
            {
                try
                {
                    // Nadpisanie XmlRoot z dynamicznym namespace
                    var rootAttribute = new XmlRootAttribute("Faktura") { Namespace = ns };
                    var serializer = new XmlSerializer(typeof(KSeFInvoiceXml), rootAttribute);
                    using var reader = new StringReader(xml);
                    if (serializer.Deserialize(reader) is KSeFInvoiceXml result)
                    {
                        _logger.LogInformation("Pomyślnie sparsowano XML faktury z namespace {Namespace}", ns);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Nie udało się sparsować XML z namespace {Namespace}, próbowanie kolejnego", ns);
                }
            }

            // Próba bez namespace
            try
            {
                var genericSerializer = new XmlSerializer(typeof(KSeFInvoiceXml));
                using var genericReader = new StringReader(xml);
                if (genericSerializer.Deserialize(genericReader) is KSeFInvoiceXml result)
                {
                    _logger.LogInformation("Pomyślnie sparsowano XML faktury bez namespace");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Nie udało się sparsować XML bez namespace");
            }

            // Jeśli wszystkie próby zawiodły, spróbuj usunąć prefiksy namespace (dla faktur z tns: itp.)
            _logger.LogWarning("Standardowe parsowanie nie powiodło się, próbując usunąć prefiksy namespace");
            var cleanedXml = RemoveNamespacePrefixes(xml);

            foreach (var ns in namespaces)
            {
                try
                {
                    // Nadpisanie XmlRoot z dynamicznym namespace
                    var rootAttribute = new XmlRootAttribute("Faktura") { Namespace = ns };
                    var serializer = new XmlSerializer(typeof(KSeFInvoiceXml), rootAttribute);
                    using var reader = new StringReader(cleanedXml);
                    if (serializer.Deserialize(reader) is KSeFInvoiceXml result)
                    {
                        _logger.LogInformation(
                            "Pomyślnie sparsowano XML faktury z namespace {Namespace} po usunięciu prefiksów", ns);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Nie udało się sparsować oczyszczonego XML z namespace {Namespace}", ns);
                }
            }

            // Ostateczna próba bez namespace po czyszczeniu
            try
            {
                var cleanedGenericSerializer = new XmlSerializer(typeof(KSeFInvoiceXml));
                using var cleanedGenericReader = new StringReader(cleanedXml);
                if (cleanedGenericSerializer.Deserialize(cleanedGenericReader) is KSeFInvoiceXml result)
                {
                    _logger.LogInformation("Pomyślnie sparsowano oczyszczony XML faktury bez namespace");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nie udało się sparsować oczyszczonego XML bez namespace");
            }

            _logger.LogError("Wszystkie próby parsowania XML faktury KSeF nie powiodły się");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas parsowania XML faktury KSeF");
            return null;
        }
    }

    /// <summary>
    /// Usuwa prefiksy namespace z elementów XML (np. tns:Faktura -> Faktura)
    /// XmlSerializer nie poprawnie radzi sobie z prefiksami namespace
    /// </summary>
    private string RemoveNamespacePrefixes(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        try
        {
            // Formatuj XML dla lepszej czytelności i parsowania
            var formattedXml = FormatXml(xml);
            
            var doc = new XmlDocument();
            doc.LoadXml(formattedXml);

            // Rekurencyjnie usuń prefiksy ze wszystkich węzłów
            if (doc.DocumentElement != null)
            {
                RemovePrefixesFromNode(doc.DocumentElement);
            }
            else
            {
                _logger.LogWarning("Dokument XML nie zawiera elementu głównego");
                return xml;
            }

            // Zachowaj deklarację XML ale usuń xmlns atrybuty
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                doc.Save(writer);
            }

            var result = sb.ToString();

            // Usuń xmlns atrybuty które mogą zostać
            result = XmlnsPrefixedNamespaceRegex().Replace(result, "");
            result = XmlnsDefaultNamespaceRegex().Replace(result, "");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nie udało się usunąć prefiksów namespace z XML, zwracam oryginalny XML");
            return xml;
        }
    }

    /// <summary>
    /// Formatuje XML dla lepszej czytelności i parsowania
    /// </summary>
    private string FormatXml(string xml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Nie udało się sformatować XML, używam oryginalnego");
            return xml;
        }
    }

    private static void RemovePrefixesFromNode(XmlNode node)
    {
        if (node == null) return;

        // Usuń prefiks z nazwy węzła poprzez klonowanie
        if (!string.IsNullOrEmpty(node.Prefix))
        {
            if (node.ParentNode != null && node.NodeType == XmlNodeType.Element)
            {
                if (node.OwnerDocument != null)
                {
                    var newNode = node.OwnerDocument.CreateElement(node.LocalName);
                    // Kopiuj atrybuty
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name.StartsWith("xmlns"))
                            continue; // Pomiń xmlns atrybuty

                        var newAttr = node.OwnerDocument.CreateAttribute(attr.LocalName);
                        newAttr.Value = attr.Value;
                        newNode.Attributes.Append(newAttr);
                    }

                    // Kopiuj zawartość
                    newNode.InnerXml = node.InnerXml;
                    // Zamień węzły
                    node.ParentNode.ReplaceChild(newNode, node);
                }
            }
        }

        // Rekurencyjnie przetwórz dzieci (wstecz, ponieważ ReplaceChild zmienia kolekcję)
        if (node.NodeType == XmlNodeType.Element)
        {
            var children = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                    children.Add(child);
            }

            foreach (var child in children)
            {
                RemovePrefixesFromNode(child);
            }
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
            nsManager.AddNamespace("fa4", "http://crd.gov.pl/wzor/2025/06/25/13775/");
            nsManager.AddNamespace("fa", "http://crd.gov.pl/wzor/2023/06/29/12648/");
            nsManager.AddNamespace("fa2", "http://crd.gov.pl/wzor/2021/11/29/11089/");

            // Próba z różnymi namespace'ami
            var grossNode = doc.SelectSingleNode("//fa4:P_15", nsManager)
                            ?? doc.SelectSingleNode("//fa:P_15", nsManager)
                            ?? doc.SelectSingleNode("//fa2:P_15", nsManager)
                            ?? doc.SelectSingleNode("//*[local-name()='P_15']");

            var invoiceNumberNode = doc.SelectSingleNode("//fa4:P_2", nsManager)
                                    ?? doc.SelectSingleNode("//fa:P_2", nsManager)
                                    ?? doc.SelectSingleNode("//fa2:P_2", nsManager)
                                    ?? doc.SelectSingleNode("//*[local-name()='P_2']");

            var invoiceDateNode = doc.SelectSingleNode("//fa4:P_1", nsManager)
                                  ?? doc.SelectSingleNode("//fa:P_1", nsManager)
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

    private static decimal ExtractNetAmount(XmlDocument doc, XmlNamespaceManager nsManager)
    {
        decimal sum = 0;
        var netFields = new[] { "P_13_1", "P_13_2", "P_13_3", "P_13_4", "P_13_5", "P_13_6_1" };

        foreach (var field in netFields)
        {
            var node = doc.SelectSingleNode($"//fa4:{field}", nsManager)
                       ?? doc.SelectSingleNode($"//fa:{field}", nsManager)
                       ?? doc.SelectSingleNode($"//fa2:{field}", nsManager)
                       ?? doc.SelectSingleNode($"//*[local-name()='{field}']");

            if (node != null && decimal.TryParse(node.InnerText, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    [GeneratedRegex("""
                    \s+xmlns:\w+="[^"]*"
                    """)]
    private static partial Regex XmlnsPrefixedNamespaceRegex();

    [GeneratedRegex("""
                    \s+xmlns="[^"]*"
                    """)]
    private static partial Regex XmlnsDefaultNamespaceRegex();
}