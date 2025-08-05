using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees;

public record DeleteEmployeeReminderCommand(Guid EmployeeId, Guid ReminderId) : IRequest<EmptyBaseResponse>;

public class DeleteEmployeeReminderCommandValidator : AbstractValidator<DeleteEmployeeReminderCommand>
{
    public DeleteEmployeeReminderCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.ReminderId).NotEmpty();
    }
}

public class DeleteEmployeeReminderCommandHandler : IRequestHandler<DeleteEmployeeReminderCommand, EmptyBaseResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;

    public DeleteEmployeeReminderCommandHandler(IEmployeeRepository employeeRepository,
        IEmployeeReminderRepository employeeReminderRepository)
    {
        _employeeRepository = employeeRepository;
        _employeeReminderRepository = employeeReminderRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteEmployeeReminderCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.EmployeeId), cancellationToken);
        var reminder =
            await _employeeReminderRepository.GetAsync(new EmployeeReminderByIdSpec(request.ReminderId),
                cancellationToken);

        if (employee.Id != reminder.EmployeeId)
        {
            throw DomainException.RecordNotFound("Przypomnienie");
        }

        await _employeeReminderRepository.DeleteAsync(reminder, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class EmployeeReminderByIdSpec : BaseSpecification<EmployeeReminderEntity>,
    ISingleResultSpecification<EmployeeReminderEntity>
{
    public EmployeeReminderByIdSpec(Guid reminderId)
    {
        EnsureExists();
        Query.Where(t => t.Id == reminderId);
    }
}