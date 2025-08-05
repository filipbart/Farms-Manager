using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees;

public class UpdateEmployeeData
{
    public Guid FarmId { get; init; }
    public string FullName { get; init; }
    public string Position { get; init; }
    public string ContractType { get; init; }
    public decimal Salary { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public EmployeeStatus Status { get; init; }
    public string Comment { get; init; }
}

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeData Data) : IRequest<EmptyBaseResponse>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;

    public UpdateEmployeeCommandHandler(IUserDataResolver userDataResolver, IEmployeeRepository employeeRepository)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.Id), cancellationToken);
        employee.Update(request.Data.FarmId,
            request.Data.FullName,
            request.Data.Position,
            request.Data.ContractType,
            request.Data.Salary,
            request.Data.Status,
            request.Data.StartDate,
            request.Data.EndDate,
            request.Data.Comment);

        employee.SetModified(userId);

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();


        RuleFor(x => x.Data.FarmId).NotEmpty();

        RuleFor(x => x.Data.FullName)
            .NotEmpty();

        RuleFor(x => x.Data.Position)
            .NotEmpty();

        RuleFor(x => x.Data.ContractType)
            .NotEmpty();

        RuleFor(x => x.Data.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Wynagrodzenie nie może być ujemne");

        RuleFor(x => x.Data.StartDate)
            .NotEmpty();

        RuleFor(x => x.Data.EndDate)
            .GreaterThan(x => x.Data.StartDate)
            .When(x => x.Data.EndDate.HasValue)
            .WithMessage("Data zakończenia musi być późniejsza niż data rozpoczęcia");

        RuleFor(x => x.Data.Status)
            .IsInEnum();
    }
}