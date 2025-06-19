using System.Net;
using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class UserNotFoundDomainException : DomainException
{
    public UserNotFoundDomainException(Exception ex) : base(ex)
    {
    }

    public override string Description => DomainExceptionDescription.UserNotFound;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}