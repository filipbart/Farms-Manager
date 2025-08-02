using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

public class EmployeeFileEntity : Entity
{
    public Guid EmployeeId { get; init; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string ContentType { get; private set; }

    public virtual EmployeeEntity Employee { get; init; }

    protected EmployeeFileEntity()
    {
    }

    public static EmployeeFileEntity CreateNew(
        Guid employeeId,
        string fileName,
        string filePath,
        string contentType,
        Guid? userId = null)
    {
        return new EmployeeFileEntity
        {
            EmployeeId = employeeId,
            FileName = fileName,
            FilePath = filePath,
            ContentType = contentType,
            CreatedBy = userId
        };
    }
}