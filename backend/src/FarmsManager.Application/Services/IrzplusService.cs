using System.Text;
using FarmsManager.Application.Common.Configurations;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Irzplus;
using FarmsManager.Application.Models.Irzplus.Common;
using FarmsManager.Application.Models.Irzplus.Dispositions;
using FarmsManager.Application.Models.Irzplus.Queries;
using FarmsManager.Application.Models.Irzplus.ZZSSD;
using FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;
using FarmsManager.Shared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
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
        var response = await httpClient.PostAsync("drob/dokument/api/prod/zzssd", jsonContent, ct);

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
        var response = await httpClient.PostAsync("drob/dokument/api/prod/zzssd", jsonContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Błąd podczas wysyłania dyspozycji do IRZplus: {response.StatusCode}\n{errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync(ct);
        return responseString.ParseJsonString<ZlozenieDyspozycjiResponse>();
    }

    public async Task<ZlozenieDyspozycjiResponse> SendFallenStocksAsync(IList<FallenStockEntity> fallenStocks,
        CancellationToken ct = default)
    {
        var authData = await AuthorizeToIrzplusAsync();

        var firstItem = fallenStocks.First();
        var mappedFallenStocks = fallenStocks.Select(fs => new FallenStocksIrzPlusDisposition(fs)).ToList();
        var dispositionZzssd = MapDispositionZzssd(mappedFallenStocks,
            firstItem.Type == FallenStockType.FallCollision
                ? IrzPlusDispositionType.FallenStocks
                : IrzPlusDispositionType.EndCycle);

        var dispositionJson = dispositionZzssd.ToJsonStringWithNulls();

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Options.Value.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authData.AccessToken}");

        var jsonContent = new StringContent(dispositionJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("drob/dokument/api/prod/zzssd", jsonContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Błąd podczas wysyłania dyspozycji do IRZplus: {response.StatusCode}\n{errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync(ct);
        return responseString.ParseJsonString<ZlozenieDyspozycjiResponse>();
    }

    public async Task<PobieranieZwierzatApiResponse> GetFlockAsync(FarmEntity farm, CancellationToken ct = default)
    {
        var authData = await AuthorizeToIrzplusAsync();

        var realproducerNumber = farm.ProducerNumber.Split("-")[0];
        var query = new PobieranieZwierzatApiRequest
        {
            NumerProducenta = realproducerNumber.Trim(),
            NumerDzialalnosci = farm.ProducerNumber,
            Gatunek = GatunekDrobiu.Kury.GetEnumMemberValue(),
            NumerPartiiDrobiu = farm.ProducerNumber,
            StanDanychNaDzien = DateOnly.FromDateTime(DateTime.Now)
        };

        var queryParams = new Dictionary<string, string>
        {
            { "numerProducenta", query.NumerProducenta },
            { "numerDzialalnosci", query.NumerDzialalnosci },
            { "gatunek", query.Gatunek },
            { "numerPartiiDrobiu", query.NumerPartiiDrobiu },
            { "stanDanychNaDzien", query.StanDanychNaDzien.ToString("O") }
        };

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Options.Value.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authData.AccessToken}");

        const string path = "drob/zwierze/api/prod/drob";
        var uri = QueryHelpers.AddQueryString(path, queryParams);

        var response = await httpClient.GetAsync(uri, ct);

        var responseString = await response.Content.ReadAsStringAsync(ct);
        return responseString.ParseJsonString<PobieranieZwierzatApiResponse>();
    }


    private static DyspozycjaZZSSD MapDispositionZzssd<T>(IList<T> items, IrzPlusDispositionType type)
        where T : IIrzPlusDisposition
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("Lista nie może być pusta");
        }

        var firstItem = items[0];
        var isInsertion = type == IrzPlusDispositionType.Insertion;

        var eventTypeDto = type switch
        {
            IrzPlusDispositionType.Sale => TypZdarzeniaDrobiu.Wybycie.ToKodOpisDto(),
            IrzPlusDispositionType.FallenStocks => TypZdarzeniaDrobiu.Padniecie.ToKodOpisDto(),
            IrzPlusDispositionType.EndCycle => TypZdarzeniaDrobiu.ZamkniecieCyklu.ToKodOpisDto(),
            _ => TypZdarzeniaDrobiu.Przybycie.ToKodOpisDto()
        };

        var totalQuantity = items.Sum(i => i.Quantity);
        var producerNumber = (isInsertion ? firstItem.DoDzialalnosci : firstItem.ZDzialalnosci).Split('-')[0];
        var destinationActivity = (type == IrzPlusDispositionType.EndCycle) ? null : firstItem.DoDzialalnosci;

        var reportPositions = items
            .Select((item, index) => new PozycjaZZSSDDTO
            {
                Lp = index + 1,
                StatusPozycji = StatusPozycjiZZSSD.Zatwierdzona.GetEnumMemberValue(),
                NumerIdenPartiiDrobiu = isInsertion ? item.DoDzialalnosci : item.ZDzialalnosci,
                LiczbaDrobiuUbylo = isInsertion ? 0 : item.Quantity,
                KategoriaJajWylegowych = null,
                Budynek = new KodOpisWartosciDto { Kod = item.HenhouseCode, Opis = item.HenhouseName },
                ZDzialalnosci = item.ZDzialalnosci
            }).ToList();

        var disposition = new DyspozycjaZZSSD
        {
            NumerProducenta = producerNumber,
            Zgloszenie = new ZgloszenieZZSSDDTO
            {
                Gatunek = GatunekDrobiu.Kury.ToKodOpisDto(),
                TypZdarzenia = eventTypeDto,
                DataZdarzenia = firstItem.EventDate,
                DoDzialalnosci = destinationActivity,
                LiczbaDrobiuPrzybylo = isInsertion ? totalQuantity : 0,
                LiczbaJajWylegowychPrzybylo = 0,
                Pozycje = reportPositions
            }
        };

        return disposition;
    }
}