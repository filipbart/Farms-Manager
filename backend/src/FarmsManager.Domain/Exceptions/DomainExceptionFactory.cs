namespace FarmsManager.Domain.Exceptions;

public abstract partial class DomainException
{
    public static DomainException RecordNotFoundException(string message) => new RecordNotFoundException(message, null);
}