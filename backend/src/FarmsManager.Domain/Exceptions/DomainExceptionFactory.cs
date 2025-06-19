namespace FarmsManager.Domain.Exceptions;

public abstract partial class DomainException
{
    public static DomainException InvalidCredentials() => new InvalidCredentialsException(null);
    public static DomainException RecordNotFound(string message) => new RecordNotFoundException(message, null);
    public static DomainException Unauthorized() => new UnauthorizedException(null);
}