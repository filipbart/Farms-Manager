namespace FarmsManager.Domain.Aggregates.UserAggregate.Dtos;

public record IrzplusCredentialsDto
{
    public Guid FarmId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}