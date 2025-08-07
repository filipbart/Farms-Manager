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
    public bool AddToAdvances { get; private set; }

    public virtual FarmEntity Farm { get; private set; }

    private readonly List<EmployeeFileEntity> _files = [];

    private readonly List<EmployeeReminderEntity> _reminders = [];
    public virtual IReadOnlyCollection<EmployeeFileEntity> Files => _files.AsReadOnly();
    public virtual IReadOnlyCollection<EmployeeReminderEntity> Reminders => _reminders.AsReadOnly();

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
        bool addToAdvances,
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
            AddToAdvances = addToAdvances,
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
        string comment,
        bool addToAdvances)
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
        AddToAdvances = addToAdvances;
    }

    public void AddFile(EmployeeFileEntity file)
    {
        _files.Add(file);
    }

    public void AddReminder(EmployeeReminderEntity reminder)
    {
        _reminders.Add(reminder);
    }
}