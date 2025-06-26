using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus;

public class IrzplusAuthResponseDto
{
    [JsonPropertyName("access_token")] 
    public string AccessToken { get; set; }
    
    [JsonPropertyName("error")] 
    public string Error { get; set; }

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; }
}