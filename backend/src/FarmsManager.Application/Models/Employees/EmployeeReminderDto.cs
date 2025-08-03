namespace FarmsManager.Application.Models.Employees;

public class EmployeeReminderDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateOnly DueDate { get; init; }
    public int DaysToRemind { get; init; }
}