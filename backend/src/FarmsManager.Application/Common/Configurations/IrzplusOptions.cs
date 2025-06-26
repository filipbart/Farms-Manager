using System.ComponentModel;
using Microsoft.Extensions.Options;

namespace FarmsManager.Application.Common.Configurations;

public class IrzplusOptions : IOptions<IrzplusOptions>
{
   public string Username { get; set; }

    public string Password { get; set; }

    [DisplayName("client_id")]
    public string ClientId { get; init; }

    [DisplayName("grant_type")]
    public string GrantType { get; init; }
    public string AuthUrl { get; init; }
    public string Url { get; init; }
    
    public IrzplusOptions Value => this;
}