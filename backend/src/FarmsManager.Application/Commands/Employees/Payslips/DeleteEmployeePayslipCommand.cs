using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees.Payslips;

public record DeleteEmployeePayslipCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteEmployeePayslipCommandValidator : AbstractValidator<DeleteEmployeePayslipCommand>
{
    public DeleteEmployeePayslipCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID wpisu wypłaty jest wymagane.");
    }
}

public class DeleteEmployeePayslipCommandHandler : IRequestHandler<DeleteEmployeePayslipCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;

    public DeleteEmployeePayslipCommandHandler(IUserDataResolver userDataResolver,
        IEmployeePayslipRepository employeePayslipRepository)
    {
        _userDataResolver = userDataResolver;
        _employeePayslipRepository = employeePayslipRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteEmployeePayslipCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var payslipToDelete =
            await _employeePayslipRepository.GetAsync(new GetPayslipByIdSpec(request.Id), cancellationToken);
        payslipToDelete.Delete(userId);

        await _employeePayslipRepository.UpdateAsync(payslipToDelete, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}