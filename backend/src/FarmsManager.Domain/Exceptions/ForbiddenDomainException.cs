using System.Net;
using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class ForbiddenDomainException: DomainException
{
    public ForbiddenDomainException(Exception ex) : base(ex)
    {
    }

    public override string Description => DomainExceptionDescription.Forbidden;
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}