using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

public class EmployeeEntity : Entity
{
    public Guid FarmId { get; private set; }
    public string FullName { get; private set; }
    public string Position { get; private set; }
    public string ContractType { get; private set; }
    public decimal Salary { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public EmployeeStatus Status { get; private set; }
    public string Comment { get; private set; }

    public virtual FarmEntity Farm { get; private set; }
    public virtual ICollection<EmployeeFileEntity> Files { get; private set; } = new List<EmployeeFileEntity>();

    protected EmployeeEntity()
    {
    }

    public static EmployeeEntity CreateNew(
        Guid farmId,
        string fullName,
        string position,
        string contractType,
        decimal salary,
        DateOnly startDate,
        DateOnly? endDate,
        string comment,
        Guid? userId = null)
    {
        return new EmployeeEntity
        {
            FarmId = farmId,
            FullName = fullName,
            Position = position,
            ContractType = contractType,
            Salary = salary,
            StartDate = startDate,
            EndDate = endDate,
            Comment = comment,
            Status = EmployeeStatus.Active,
            CreatedBy = userId
        };
    }

    public void Update(
        Guid farmId,
        string fullName,
        string position,
        string contractType,
        decimal salary,
        EmployeeStatus status,
        DateOnly startDate,
        DateOnly? endDate,
        string comment)
    {
        FarmId = farmId;
        FullName = fullName;
        Position = position;
        ContractType = contractType;
        Salary = salary;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        Comment = comment;
    }

    public void AddFile(EmployeeFileEntity file)
    {
        Files.Add(file);
    }
}