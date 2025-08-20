namespace FarmsManager.Domain.Aggregates.UserAggregate.Models;

public class IrzplusCredentials
{
    public Guid FarmId { get; set; }
    public string Login { get; set; }
    public string EncryptedPassword { get; set; }
}