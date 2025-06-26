using FarmsManager.Application.Common.Configurations;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Irzplus;
using FarmsManager.Application.Models.Irzplus.Common;
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
            throw new Exception("Błąd autoryzacji do IRZplus");
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

        var firstInsertion = insertions.First();
        var dispositionZzssd = new DyspozycjaZZSSD
        {
            NumerProducenta = firstInsertion.Farm.ProducerNumber,
            Zgloszenie = new ZgloszenieZZSSDDTO
            {
                Pozycje = null,
                Gatunek = GatunekDrobiu.Kury.ToKodOpisDto(),
                DoDzialalnosci = firstInsertion.Farm.ProducerNumber,
                TypZdarzenia = TypZdarzeniaDrobiu.Przybycie.ToKodOpisDto(),
                DataZdarzenia = firstInsertion.InsertionDate,
                LiczbaDrobiuPrzybylo = insertions.Sum(t => t.Quantity),
                LiczbaJajWylegowychPrzybylo = 0,
                KodKraju = new KodOpisWartosciDto
                {
                    Kod = "PL",
                    Opis = "POLSKA"
                }
            }
        };

        List<PozycjaZZSSDDTO> items = [];

        foreach (var (insertionEntity, index) in insertions.Select((value, index) => (value, index)))
        {
            var item = new PozycjaZZSSDDTO
            {
                Lp = index + 1,
                StatusPozycji = StatusPozycjiZZSSD.Zatwierdzona,
                NumerIdenPartiiDrobiu = insertionEntity.Farm.ProducerNumber,
                LiczbaDrobiuUbylo = insertionEntity.Quantity,
                KategoriaJajWylegowych = KategoriaJajWylegowych.Miesna.ToKodOpisDto(), //TODO do weryfikacji
                Budynek = new KodOpisWartosciDto
                {
                    Kod = insertionEntity.Henhouse.Code,
                    Opis = insertionEntity.Henhouse.Name
                },
                ZDzialalnosci = insertionEntity.Hatchery.ProducerNumber
            };

            items.Add(item);
        }

        dispositionZzssd.Zgloszenie.Pozycje = items;

        var dispositionJson = dispositionZzssd.ToJsonStringWithNulls();

        return null;
    }
}