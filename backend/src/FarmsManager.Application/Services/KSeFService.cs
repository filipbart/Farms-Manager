using System.Text;
using FarmsManager.Application.Common;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Shared.Extensions;
using KSeF.Client.Api.Builders.Auth;
using KSeF.Client.Api.Builders.X509Certificates;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Authorization;
using KSeF.Client.Core.Models.Invoices;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

/// <summary>
/// Serwis do komunikacji z Krajowym Systemem e-Faktur (KSeF)
/// </summary>
public class KSeFService : IKSeFService, IService
{
    private readonly IKSeFClient _ksefClient;
    private readonly ICryptographyService _cryptographyService;
    private readonly ISignatureService _signatureService;
    private readonly ILogger<KSeFService> _logger;
    private readonly KSeFInvoiceXmlParser _xmlParser;

    // TODO: Implementacja autoryzacji - placeholder na token/sesję
    private string _sessionToken;
    private string _sessionRefreshToken;

    private const string Nip = "7394056953";

    public KSeFService(
        IKSeFClient ksefClient,
        ILogger<KSeFService> logger, 
        ISignatureService signatureService, 
        ICryptographyService cryptographyService,
        KSeFInvoiceXmlParser xmlParser)
    {
        _ksefClient = ksefClient;
        _logger = logger;
        _signatureService = signatureService;
        _cryptographyService = cryptographyService;
        _xmlParser = xmlParser;
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
                //Subject1 - Nabywca, Subject2 - Sprzedawca ale sprawdzic
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
        fileContent = fileContent.Replace("#nip#", Nip);
        fileContent = fileContent.Replace("#invoice_number#", "1/11/2025");

        var encryptionData = _cryptographyService.GetEncryptionData();

        var openOnlineSessionRequest = OpenOnlineSessionRequestBuilder.Create()
            .WithFormCode(SystemCodeHelper.GetSystemCode(SystemCodeEnum.FA3),
                SystemCodeHelper.GetSchemaVersion(SystemCodeEnum.FA3), SystemCodeHelper.GetValue(SystemCodeEnum.FA3))
            .WithEncryption(encryptionData.EncryptionInfo.EncryptedSymmetricKey,
                encryptionData.EncryptionInfo.InitializationVector)
            .Build();

        var openOnlineSessionResponse =
            await _ksefClient.OpenOnlineSessionAsync(openOnlineSessionRequest, _sessionToken, cancellationToken);

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var invoiceBytes = memoryStream.ToArray();

        var encryptedInvoice =
            _cryptographyService.EncryptBytesWithAES256(invoiceBytes, encryptionData.CipherKey,
                encryptionData.CipherIv);
        var invoiceMetadata = _cryptographyService.GetMetaData(invoiceBytes);
        var encryptedInvoiceMetadata = _cryptographyService.GetMetaData(encryptedInvoice);

        var sendOnlineInvoiceRequest = SendInvoiceOnlineSessionRequestBuilder.Create()
            .WithInvoiceHash(invoiceMetadata.HashSHA, invoiceMetadata.FileSize)
            .WithEncryptedDocumentHash(encryptedInvoiceMetadata.HashSHA, encryptedInvoiceMetadata.FileSize)
            .WithEncryptedDocumentContent(Convert.ToBase64String(encryptedInvoice))
            .Build();

        var sendInvoiceResponse = await _ksefClient.SendOnlineSessionInvoiceAsync(sendOnlineInvoiceRequest,
            openOnlineSessionResponse.ReferenceNumber, _sessionToken, cancellationToken);

        if (sendInvoiceResponse.ReferenceNumber == null)
        {
            throw new Exception("Błąd z wysłaniem faktury do KSeF");
        }

        await _ksefClient.CloseOnlineSessionAsync(openOnlineSessionResponse.ReferenceNumber, _sessionToken,
            cancellationToken);

        return sendInvoiceResponse.ReferenceNumber;
    }

    /// <summary>
    /// Zapewnia aktywną sesję z KSeF
    /// </summary>
    private async Task EnsureSessionAsync(CancellationToken cancellationToken)
    {
        if (_sessionToken.IsEmpty())
        {
            var challenge = await _ksefClient.GetAuthChallengeAsync(cancellationToken);

            // 1. Budowa AuthTokenRequest
            var authTokenRequest = AuthTokenRequestBuilder
                .Create()
                .WithChallenge(challenge.Challenge)
                .WithContext(AuthenticationTokenContextIdentifierType.Nip, Nip)
                .WithIdentifierType(AuthenticationTokenSubjectIdentifierTypeEnum.CertificateSubject)
                .Build();

            var certificate = SelfSignedCertificateForSealBuilder
                .Create()
                .WithOrganizationName("Fermy Drobiu test")
                .WithOrganizationIdentifier($"VATPL-{Nip}")
                .WithCommonName("Fermy Drobiu teścik")
                .Build();
// 2. Serializacja do XML
            var unsignedXml = authTokenRequest.SerializeToXmlString();

// 3. Podpisanie XAdES
            var signedXml = _signatureService.Sign(unsignedXml, certificate);

// 4. Wysłanie podpisanego XML
// POST /api/v2/auth/xades-signature
            var authOperationInfo = await _ksefClient.SubmitXadesAuthRequestAsync(
                signedXml,
                verifyCertificateChain: false, cancellationToken: cancellationToken // true dla produkcji
            );

            var status = await _ksefClient.GetAuthStatusAsync(authOperationInfo.ReferenceNumber,
                authOperationInfo.AuthenticationToken.Token, cancellationToken);

            if (status.Status.Code != 200)
                throw new Exception("Bład uwierzytelniania do KSeF");

            var tokens =
                await _ksefClient.GetAccessTokenAsync(authOperationInfo.AuthenticationToken.Token, cancellationToken);

            _sessionToken = tokens.AccessToken.Token;
            _sessionRefreshToken = tokens.RefreshToken.Token;
        }
    }
}