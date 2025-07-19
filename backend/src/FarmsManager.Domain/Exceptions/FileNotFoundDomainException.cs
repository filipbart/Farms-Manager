using System.Net;
using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class FileNotFoundDomainException : DomainException
{
    public FileNotFoundDomainException(Exception ex) : base(ex)
    {
    }

    public override string Description => DomainExceptionDescription.FileNotFound;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}