using System.Net;
using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(Exception ex) : base(ex)
    {
    }

    public override string Description => DomainExceptionDescription.Unauthorized;
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}