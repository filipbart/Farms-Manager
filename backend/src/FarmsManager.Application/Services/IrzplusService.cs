using System.Text;
using FarmsManager.Application.Common.Configurations;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Irzplus;
using FarmsManager.Application.Models.Irzplus.Common;
using FarmsManager.Application.Models.Irzplus.Dispositions;
using FarmsManager.Application.Models.Irzplus.ZZSSD;
using FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;
using FarmsManager.Shared.Extensions;
using Microsoft.Extensions.Options;

namespace FarmsManager.Application.Services;

public class IrzplusService : IIrzplusService
{
    private IOptions<IrzplusOptions> Options { get; set; }
    private readonly IEncryptionService _encryptionService;

    public IrzplusService(IOptions<IrzplusOptions> options, IEncryptionService encryptionService)
    {
        Options = options;
        _encryptionService = encryptionService;
    }

    public void PrepareOptions(IrzplusCredentials credentials)
    {
        var password = _encryptionService.Decrypt(credentials.EncryptedPassword);

        Options.Value.Username = credentials.Login;
        Options.Value.Password = password;
    }

    private async Task<IrzplusAuthResponseDto> AuthorizeToIrzplusAsync()
    {
        using var httpClient = new HttpClient();

        var uri = new Uri(Options.Value.AuthUrl);

        var parameters = new Dictionary<string, string>
        {
            { "username", Options.Value.Username },
            { "password", Options.Value.Password },
            { "client_id", Options.Value.ClientId },
            { "grant_type", Options.Value.GrantType }
        };

        var formData = new FormUrlEncodedContent(parameters);

        var response = await httpClient.PostAsync(uri, formData);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd autoryzacji do IRZplus: {response.StatusCode}\n{errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseModel = responseString.ParseJsonString<IrzplusAuthResponseDto>();
        if (responseModel.Error is not null)
        {
            throw new Exception(responseModel.ErrorDescription);
        }

        return responseModel;
    }

    public async Task<ZlozenieDyspozycjiResponse> SendInsertionsAsync(IList<InsertionEntity> insertions,
        CancellationToken ct)
    {
        var authData = await AuthorizeToIrzplusAsync();

        var mappedInsertions = insertions.Select(i => new InsertionIrzPlusDisposition(i)).ToList();
        var dispositionZzssd = MapDispositionZzssd(mappedInsertions, IrzPlusDispositionType.Insertion);

        var dispositionJson = dispositionZzssd.ToJsonStringWithNulls();

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Options.Value.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authData.AccessToken}");

        var jsonContent = new StringContent(dispositionJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("drob/dokument/api/test/zzssd", jsonContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Błąd podczas wysyłania dyspozycji do IRZplus: {response.StatusCode}\n{errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync(ct);
        return responseString.ParseJsonString<ZlozenieDyspozycjiResponse>();
    }


    public async Task<ZlozenieDyspozycjiResponse> SendSalesAsync(IList<SaleEntity> sales,
        CancellationToken ct = default)
    {
        var authData = await AuthorizeToIrzplusAsync();

        var mappedSales = sales.Select(s => new SaleIrzPlusDisposition(s)).ToList();
        var dispositionZzssd = MapDispositionZzssd(mappedSales, IrzPlusDispositionType.Sale);

        var dispositionJson = dispositionZzssd.ToJsonStringWithNulls();

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Options.Value.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authData.AccessToken}");

        var jsonContent = new StringContent(dispositionJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("drob/dokument/api/test/zzssd", jsonContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Błąd podczas wysyłania dyspozycji do IRZplus: {response.StatusCode}\n{errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync(ct);
        return responseString.ParseJsonString<ZlozenieDyspozycjiResponse>();
    }


    private static DyspozycjaZZSSD MapDispositionZzssd<T>(IList<T> items, IrzPlusDispositionType type)
        where T : IIrzPlusDisposition
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Lista nie może być pusta");

        var first = items.First();

        var typZdarzenia = type == IrzPlusDispositionType.Sale
            ? TypZdarzeniaDrobiu.Wybycie.ToKodOpisDto()
            : TypZdarzeniaDrobiu.Przybycie.ToKodOpisDto();

        var doDzialalnosci = type == IrzPlusDispositionType.Sale
            ? (items.First() as SaleIrzPlusDisposition)?.Sale.Slaughterhouse.ProducerNumber ?? first.ProducerNumber
            : first.ProducerNumber;

        var sumQuantity = items.Sum(i => i.Quantity);
        var realproducerNumber = first.ProducerNumber.Split("-")[0];

        var disposition = new DyspozycjaZZSSD
        {
            NumerProducenta = realproducerNumber,
            Zgloszenie = new ZgloszenieZZSSDDTO
            {
                Pozycje = null,
                Gatunek = GatunekDrobiu.Kury.ToKodOpisDto(),
                DoDzialalnosci = doDzialalnosci,
                TypZdarzenia = typZdarzenia,
                DataZdarzenia = first.EventDate,
                LiczbaDrobiuPrzybylo = sumQuantity,
                LiczbaJajWylegowychPrzybylo = 0
            }
        };

        var pozycje = items.Select((item, i) => new PozycjaZZSSDDTO
            {
                Lp = i + 1,
                StatusPozycji = StatusPozycjiZZSSD.Zatwierdzona.GetEnumMemberValue(),
                NumerIdenPartiiDrobiu = item.ProducerNumber,
                LiczbaDrobiuUbylo = item.Quantity,
                KategoriaJajWylegowych = null,
                Budynek = new KodOpisWartosciDto { Kod = item.HenhouseCode, Opis = item.HenhouseName },
                ZDzialalnosci = item.ZdDzialalnosci
            })
            .ToList();

        disposition.Zgloszenie.Pozycje = pozycje;

        return disposition;
    }
}