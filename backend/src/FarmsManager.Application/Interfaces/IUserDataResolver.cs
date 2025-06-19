namespace FarmsManager.Application.Interfaces;

public interface IUserDataResolver
{
    string GetLoginAsync();
    Guid? GetUserId();
    Guid? TryGetSessionId();
}