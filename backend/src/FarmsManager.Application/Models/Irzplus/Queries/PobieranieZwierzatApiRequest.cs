using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.Queries;

public record PobieranieZwierzatApiRequest
{
    [JsonPropertyName("numerProducenta")] public string NumerProducenta { get; set; }

    [JsonPropertyName("numerDzialalnosci")]
    public string NumerDzialalnosci { get; set; }

    [JsonPropertyName("gatunek")] public string Gatunek { get; set; }

    [JsonPropertyName("numerPartiiDrobiu")]
    public string NumerPartiiDrobiu { get; set; }

    [JsonPropertyName("stanDanychNaDzien")]
    public DateOnly StanDanychNaDzien { get; set; }
};