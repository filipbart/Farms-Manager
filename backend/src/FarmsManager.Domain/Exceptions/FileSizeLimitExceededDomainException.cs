using System.Net;

namespace FarmsManager.Domain.Exceptions;

public class FileSizeLimitExceededDomainException : DomainException
{
    public FileSizeLimitExceededDomainException(Exception ex) : base(ex)
    {
    }

    public override string Description => "Przekroczono limit rozmiaru pliku (max 10 MB)";
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
