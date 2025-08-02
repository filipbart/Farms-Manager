using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

public class EmployeeReminderEntity : Entity
{
    public Guid EmployeeId { get; init; }
    public string Title { get; private set; }
    public DateOnly DueDate { get; private set; }

    public virtual EmployeeEntity Employee { get; init; }

    protected EmployeeReminderEntity()
    {
    }

    public static EmployeeReminderEntity CreateNew(
        Guid employeeId,
        string title,
        DateOnly dueDate,
        Guid? userId = null)
    {
        return new EmployeeReminderEntity
        {
            EmployeeId = employeeId,
            Title = title,
            DueDate = dueDate,
            CreatedBy = userId
        };
    }
}