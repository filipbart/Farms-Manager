using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class BadRequestDomainException : DomainException
{
    private readonly string _message;

    public BadRequestDomainException(string message, Exception innerException) : base(innerException)
    {
        _message = message;
    }

    public override string Description => string.IsNullOrEmpty(_message) 
        ? DomainExceptionDescription.BadRequest 
        : _message;
}
