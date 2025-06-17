using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class RecordNotFoundException : DomainException
{
    private readonly string _message;

    public RecordNotFoundException(string message, Exception innerException) : base(innerException)
    {
        _message = message;
    }

    public override string Description => string.Format(DomainExceptionDescription.RecordNotFound, _message);
}