using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException(Exception ex) : base(ex)
    {
    }

    public override string Description => DomainExceptionDescription.InvalidCredentials;
}