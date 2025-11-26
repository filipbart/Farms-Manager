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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FarmsManager.Application.Services;

/// <summary>
/// Serwis do komunikacji z Krajowym Systemem e-Faktur (KSeF)
/// </summary>
public class KSeFService : IKSeFService, IService
{
    private readonly IKSeFClient _ksefClient;
    private readonly ISignatureService _signatureService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KSeFService> _logger;

    // TODO: Implementacja autoryzacji - placeholder na token/sesję
    private string _sessionToken;
    private string _sessionRefreshToken;

    private const string Nip = "7394056953";

    public KSeFService(
        IKSeFClient ksefClient,
        IConfiguration configuration,
        ILogger<KSeFService> logger, ISignatureService signatureService)
    {
        _ksefClient = ksefClient;
        _configuration = configuration;
        _logger = logger;
        _signatureService = signatureService;
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

            var filters = new InvoiceQueryFilters();
            var invoicesMetadata =
                await _ksefClient.QueryInvoiceMetadataAsync(filters, _sessionToken, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);


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

            // TODO: Implementacja pobierania szczegółów faktury
            // Wymagane:
            // 1. Wywołanie _ksefClient.InvoiceGetAsync()
            // 2. Parsowanie XML faktury
            // 3. Mapowanie na KSeFInvoiceDetails

            _logger.LogWarning("Pobieranie szczegółów faktury {ReferenceNumber} wymaga implementacji",
                invoiceReferenceNumber);

            return new KSeFInvoiceDetails
            {
                ReferenceNumber = invoiceReferenceNumber
            };
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

            // TODO: Implementacja pobierania XML faktury
            _logger.LogWarning("Pobieranie XML faktury {ReferenceNumber} wymaga implementacji",
                invoiceReferenceNumber);

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania XML faktury {ReferenceNumber} z KSeF",
                invoiceReferenceNumber);
            throw;
        }
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