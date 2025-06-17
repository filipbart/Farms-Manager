using System.Net;

namespace FarmsManager.Domain.Exceptions;

[Serializable]
public abstract partial class DomainException : Exception
{
    private readonly Exception _innerException;

    protected DomainException(Exception innerException)
    {
        _innerException = innerException;
    }

    public Exception GetInnerException() => _innerException;

    /// <summary>
    /// Nazwa wyjątku
    /// </summary>
    public string Name => GetType().Name.Replace("DomainException", "");

    /// <summary>
    /// Opis wyjątku
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Kod HTTP wyjątku
    /// </summary>
    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public override string ToString() => _innerException.ToString();
}