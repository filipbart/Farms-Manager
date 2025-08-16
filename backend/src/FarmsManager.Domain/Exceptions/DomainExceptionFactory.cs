namespace FarmsManager.Domain.Exceptions;

public abstract partial class DomainException
{
    public static DomainException InvalidCredentials() => new InvalidCredentialsException(null);
    public static DomainException RecordNotFound(string message) => new RecordNotFoundException(message, null);
    public static DomainException Unauthorized() => new UnauthorizedException(null);
    public static DomainException UserNotFound() => new UserNotFoundDomainException(null);
    public static DomainException InsertionExists(string henhouseName) => new InsertionExistsDomainException(henhouseName, null);
    public static DomainException FileNotFound() => new FileNotFoundDomainException(null);
    public static DomainException Forbidden() => new ForbiddenDomainException(null);
}