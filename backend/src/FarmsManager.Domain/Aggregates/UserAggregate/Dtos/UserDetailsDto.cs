namespace FarmsManager.Domain.Aggregates.UserAggregate.Dtos;

public class UserDetailsDto
{
    public string Login { get; init; }
    public string Name { get; init; }
    public IrzplusCredentialsDto IrzplusCredentials { get; init; }
}