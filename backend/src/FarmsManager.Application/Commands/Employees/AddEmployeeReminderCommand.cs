using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees;

public record AddEmployeeReminderData
{
    public string Title { get; init; }
    public DateOnly DueDate { get; init; }
}

public record AddEmployeeReminderCommand(Guid EmployeeId, AddEmployeeReminderData Data) : IRequest<EmptyBaseResponse>;

public class AddEmployeeReminderValidator : AbstractValidator<AddEmployeeReminderCommand>
{
    public AddEmployeeReminderValidator()
    {
        RuleFor(t => t.Data.Title).NotEmpty().WithMessage("Wiadomość przypomnienia jest wymagana");
        RuleFor(t => t.Data.DueDate)
            .NotEmpty().WithMessage("Data przypomnienia jest wymagana.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data przypomnienia nie może być z przeszłości.");
    }
}

public class AddEmployeeReminderCommandHandler : IRequestHandler<AddEmployeeReminderCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;

    public AddEmployeeReminderCommandHandler(IUserDataResolver userDataResolver, IEmployeeRepository employeeRepository)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddEmployeeReminderCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.EmployeeId), cancellationToken);

        employee.AddReminder(EmployeeReminderEntity.CreateNew(employee.Id, request.Data.Title, request.Data.DueDate,
            userId));
        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}