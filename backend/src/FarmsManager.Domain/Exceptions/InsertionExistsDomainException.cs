using FarmsManager.Domain.Resources;

namespace FarmsManager.Domain.Exceptions;

public class InsertionExistsDomainException : DomainException
{
    private readonly string _henhouseName;

    public InsertionExistsDomainException(string henhouseName, Exception ex) : base(ex)
    {
        _henhouseName = henhouseName;
    }

    public override string Description => string.Format(DomainExceptionDescription.InsertionExists, _henhouseName);
}