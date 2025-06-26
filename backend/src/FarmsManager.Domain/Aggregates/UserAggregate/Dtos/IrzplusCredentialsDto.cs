namespace FarmsManager.Domain.Aggregates.UserAggregate.Dtos;

public record IrzplusCredentialsDto
{
    public string Login { get; set; }
    public string Password { get; set; }
}