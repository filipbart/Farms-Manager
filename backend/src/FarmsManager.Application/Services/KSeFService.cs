using System.Text;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Shared.Extensions;
using KSeF.Client.Api.Builders.Auth;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Authorization;
using KSeF.Client.Core.Models.Invoices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

/// <summary>
/// Serwis do komunikacji z Krajowym Systemem e-Faktur (KSeF)
/// </summary>
public class KSeFService : IKSeFService
{
    private readonly IKSeFClient _ksefClient;
    private readonly ICryptographyService _cryptographyService;
    private readonly ILogger<KSeFService> _logger;
    private readonly IKSeFInvoiceXmlParser _xmlParser;

    private string _sessionToken;
    private string _sessionRefreshToken;

    private readonly string _nip;
    private readonly string _tokenKSeF;

    public KSeFService(
        IKSeFClient ksefClient,
        ILogger<KSeFService> logger,
        ICryptographyService cryptographyService,
        IKSeFInvoiceXmlParser xmlParser,
        IConfiguration configuration)
    {
        _ksefClient = ksefClient;
        _logger = logger;
        _cryptographyService = cryptographyService;
        _xmlParser = xmlParser;
        _tokenKSeF = configuration.GetSection("KSeFDev").GetValue<string>("Token");
        _nip = configuration.GetSection("KSeFDev").GetValue<string>("NIP");
    }

    /// <summary>
    /// Pobiera faktury z KSeF na podstawie kryteriów wyszukiwania
    /// </summary>
    public async Task<KSeFInvoicesResponse> GetInvoicesAsync(
        KSeFInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await EnsureSessionAsync(cancellationToken);

            var now = DateTime.Now;
            var yearBefore = now.AddYears(-1);
            var filters = new InvoiceQueryFilters
            {
                //Subject1 - my jako sprzedawca, Subject2 - my jako kupujący
                SubjectType = SubjectType.Subject1,
                DateRange = new DateRange
                {
                    DateType = DateType.PermanentStorage,
                    From = yearBefore
                }
            };
            var invoicesMetadata =
                await _ksefClient.QueryInvoiceMetadataAsync(filters, _sessionToken, request.PageNumber,
                    request.PageSize, cancellationToken: cancellationToken);


            var responseList = invoicesMetadata.Invoices.Select(t => new KSeFInvoiceItem
            {
                ReferenceNumber = t.KsefNumber,
                InvoiceNumber = t.InvoiceNumber,
                InvoiceDate = t.InvoicingDate,
                GrossAmount = t.GrossAmount,
                NetAmount = t.NetAmount,
                VatAmount = t.VatAmount,
                SellerNip = t.Seller.Nip,
                SellerName = t.Seller.Name,
                BuyerNip = t.Buyer.Identifier.Value,
                BuyerName = t.Buyer.Name,
                ReceivedDate = t.AcquisitionDate
            }).ToList();

            return new KSeFInvoicesResponse
            {
                Invoices = responseList,
                TotalCount = invoicesMetadata.Invoices.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania faktur z KSeF");
            throw;
        }
    }

    /// <summary>
    /// Pobiera WSZYSTKIE faktury z KSeF do synchronizacji,
    /// łącząc wyniki dla Subject1 (sprzedawca) i Subject2 (kupujący),
    /// z pełnym przejściem po stronach.
    /// </summary>
    public async Task<List<KSeFInvoiceSyncItem>> GetInvoicesForSyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            await EnsureSessionAsync(cancellationToken);

            var now = DateTime.Now;
            var yearBefore = now.AddMonths(-2);
            var dateRange = new DateRange
            {
                DateType = DateType.PermanentStorage,
                From = yearBefore
            };

            const int pageSize = 100;

            // Pobranie faktur jako sprzedawca (Sales)
            var salesInvoices = await GetAllInvoicesForSubjectAsync(
                SubjectType.Subject1, dateRange, pageSize, KSeFInvoiceItemDirection.Sales, cancellationToken);

            // Pobranie faktur jako kupujący (Purchase)
            var purchaseInvoices = await GetAllInvoicesForSubjectAsync(
                SubjectType.Subject2, dateRange, pageSize, KSeFInvoiceItemDirection.Purchase, cancellationToken);

            // Połączenie wyników z deduplikacją po KsefNumber
            var combined = salesInvoices
                .Concat(purchaseInvoices)
                .Where(inv => !string.IsNullOrWhiteSpace(inv.KsefNumber))
                .DistinctBy(inv => inv.KsefNumber, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(inv => inv.InvoiceDate)
                .ToList();

            return combined;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania faktur z KSeF do synchronizacji");
            throw;
        }
    }

    private async Task<List<KSeFInvoiceSyncItem>> GetAllInvoicesForSubjectAsync(
        SubjectType subjectType,
        DateRange dateRange,
        int pageSize,
        KSeFInvoiceItemDirection direction,
        CancellationToken cancellationToken)
    {
        var all = new List<KSeFInvoiceSyncItem>();
        var page = 0;
        var filters = new InvoiceQueryFilters
        {
            SubjectType = subjectType,
            DateRange = dateRange
        };

        while (true)
        {
            try
            {
                var pageResult = await _ksefClient.QueryInvoiceMetadataAsync(
                    filters,
                    _sessionToken,
                    page,
                    pageSize,
                    cancellationToken: cancellationToken);

                var items = pageResult?.Invoices?.ToList() ?? [];

                if (items.Count == 0)
                    break;

                // Konwertuj InvoiceSummary na KSeFInvoiceSyncItem
                var syncItems = items.Select(inv => new KSeFInvoiceSyncItem
                {
                    KsefNumber = inv.KsefNumber,
                    InvoiceNumber = inv.InvoiceNumber,
                    InvoiceDate = DateOnly.FromDateTime(inv.InvoicingDate.LocalDateTime),
                    GrossAmount = inv.GrossAmount,
                    NetAmount = inv.NetAmount,
                    VatAmount = inv.VatAmount,
                    SellerNip = inv.Seller?.Nip ?? string.Empty,
                    SellerName = inv.Seller?.Name ?? string.Empty,
                    BuyerNip = inv.Buyer?.Identifier?.Value ?? string.Empty,
                    BuyerName = inv.Buyer?.Name ?? string.Empty,
                    Direction = direction,
                    InvoiceType = inv.InvoiceType
                });

                all.AddRange(syncItems);

                if (items.Count < pageSize)
                    break;

                page++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd podczas pobierania strony {Page} faktur z KSeF dla {SubjectType}. " +
                    "Prawdopodobnie błąd po stronie systemu KSeF",
                    page, subjectType);
                throw;
            }
        }

        return all;
    }

    /// <summary>
    /// Pobiera szczegóły pojedynczej faktury z KSeF
    /// </summary>
    public async Task<KSeFInvoiceDetails> GetInvoiceDetailsAsync(
        string invoiceReferenceNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            await EnsureSessionAsync(cancellationToken);

            // Pobierz XML faktury
            var invoiceXml = await _ksefClient.GetInvoiceAsync(
                invoiceReferenceNumber,
                _sessionToken,
                cancellationToken);

            if (string.IsNullOrEmpty(invoiceXml))
            {
                _logger.LogWarning("Nie udało się pobrać XML faktury {ReferenceNumber}", invoiceReferenceNumber);
                return new KSeFInvoiceDetails { ReferenceNumber = invoiceReferenceNumber };
            }

            // Parsuj XML do modelu
            var parsedInvoice = _xmlParser.ParseInvoiceXml(invoiceXml);

            if (parsedInvoice == null)
            {
                _logger.LogWarning("Nie udało się sparsować XML faktury {ReferenceNumber}", invoiceReferenceNumber);
                return new KSeFInvoiceDetails { ReferenceNumber = invoiceReferenceNumber };
            }

            return _xmlParser.ToInvoiceDetails(parsedInvoice, invoiceReferenceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania szczegółów faktury {ReferenceNumber} z KSeF",
                invoiceReferenceNumber);
            throw;
        }
    }

    /// <summary>
    /// Pobiera XML faktury z KSeF
    /// </summary>
    public async Task<string> GetInvoiceXmlAsync(
        string invoiceReferenceNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            await EnsureSessionAsync(cancellationToken);

            var invoiceXml = await _ksefClient.GetInvoiceAsync(
                invoiceReferenceNumber,
                _sessionToken,
                cancellationToken);

            _logger.LogInformation("Pobrano XML faktury {ReferenceNumber}, rozmiar: {Size} bajtów",
                invoiceReferenceNumber, invoiceXml?.Length ?? 0);

            return invoiceXml;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania XML faktury {ReferenceNumber} z KSeF",
                invoiceReferenceNumber);
            throw;
        }
    }

    public async Task<string> SendTestInvoiceAsync(string fileContent, CancellationToken cancellationToken)
    {
        await EnsureSessionAsync(cancellationToken);

        var preparedContent = fileContent
            .Replace("#nip#", _nip)
            .Replace("#invoice_number#", "1/12/2025");

        var encryptionData = _cryptographyService.GetEncryptionData();

        var openOnlineSessionRequest = OpenOnlineSessionRequestBuilder.Create()
            .WithFormCode(
                SystemCodeHelper.GetSystemCode(SystemCodeEnum.FA3),
                SystemCodeHelper.GetSchemaVersion(SystemCodeEnum.FA3),
                SystemCodeHelper.GetValue(SystemCodeEnum.FA3))
            .WithEncryption(
                encryptionData.EncryptionInfo.EncryptedSymmetricKey,
                encryptionData.EncryptionInfo.InitializationVector)
            .Build();

        var openOnlineSessionResponse = await _ksefClient.OpenOnlineSessionAsync(
            openOnlineSessionRequest, _sessionToken, cancellationToken);

        var invoiceBytes = Encoding.UTF8.GetBytes(preparedContent);
        var encryptedInvoice = _cryptographyService.EncryptBytesWithAES256(
            invoiceBytes, encryptionData.CipherKey, encryptionData.CipherIv);

        var invoiceMetadata = _cryptographyService.GetMetaData(invoiceBytes);
        var encryptedInvoiceMetadata = _cryptographyService.GetMetaData(encryptedInvoice);

        var sendOnlineInvoiceRequest = SendInvoiceOnlineSessionRequestBuilder.Create()
            .WithInvoiceHash(invoiceMetadata.HashSHA, invoiceMetadata.FileSize)
            .WithEncryptedDocumentHash(encryptedInvoiceMetadata.HashSHA, encryptedInvoiceMetadata.FileSize)
            .WithEncryptedDocumentContent(Convert.ToBase64String(encryptedInvoice))
            .Build();

        var sendInvoiceResponse = await _ksefClient.SendOnlineSessionInvoiceAsync(
            sendOnlineInvoiceRequest,
            openOnlineSessionResponse.ReferenceNumber,
            _sessionToken,
            cancellationToken);

        if (sendInvoiceResponse.ReferenceNumber == null)
            throw new InvalidOperationException("Błąd z wysłaniem faktury do KSeF");

        await _ksefClient.CloseOnlineSessionAsync(
            openOnlineSessionResponse.ReferenceNumber, _sessionToken, cancellationToken);

        return sendInvoiceResponse.ReferenceNumber;
    }

    /// <summary>
    /// Zapewnia aktywną sesję z KSeF
    /// </summary>
    private async Task EnsureSessionAsync(CancellationToken cancellationToken)
    {
        if (!_sessionToken.IsEmpty())
            return;

        var challenge = await _ksefClient.GetAuthChallengeAsync(cancellationToken);
        var timestamp = challenge.Timestamp.ToUnixTimeMilliseconds();

        var tokenWithTimestamp = $"{_tokenKSeF}|{timestamp}";
        var tokenBytes = Encoding.UTF8.GetBytes(tokenWithTimestamp);
        var encryptedBytes = _cryptographyService.EncryptKsefTokenWithRSAUsingPublicKey(tokenBytes);
        var encryptedTokenBase64 = Convert.ToBase64String(encryptedBytes);

        var authRequest = AuthKsefTokenRequestBuilder
            .Create()
            .WithChallenge(challenge.Challenge)
            .WithContext(AuthenticationTokenContextIdentifierType.Nip, _nip)
            .WithEncryptedToken(encryptedTokenBase64)
            .Build();

        var signatureResponse = await _ksefClient.SubmitKsefTokenAuthRequestAsync(authRequest, cancellationToken);

        var status = await _ksefClient.GetAuthStatusAsync(
            signatureResponse.ReferenceNumber,
            signatureResponse.AuthenticationToken.Token,
            cancellationToken);

        // Status 100 oznacza, że uwierzytelnianie jest w toku - czekamy w pętli
        while (status.Status.Code == 100)
        {
            await Task.Delay(1000, cancellationToken);
            status = await _ksefClient.GetAuthStatusAsync(
                signatureResponse.ReferenceNumber,
                signatureResponse.AuthenticationToken.Token,
                cancellationToken);
        }

        if (status.Status.Code != 200)
            throw new InvalidOperationException("Błąd uwierzytelniania do KSeF");

        var tokens = await _ksefClient.GetAccessTokenAsync(
            signatureResponse.AuthenticationToken.Token, cancellationToken);

        _sessionToken = tokens.AccessToken.Token;
        _sessionRefreshToken = tokens.RefreshToken.Token;
    }
}