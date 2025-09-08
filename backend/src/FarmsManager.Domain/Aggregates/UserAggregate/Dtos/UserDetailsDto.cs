namespace FarmsManager.Domain.Aggregates.UserAggregate.Dtos;

public class UserDetailsDto
{
    public string Login { get; init; }
    public string Name { get; init; }
    public string AvatarPath { get; init; }
    public List<IrzplusCredentialsDto> IrzplusCredentials { get; init; }
}