using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Employees;

public record AddEmployeeData
{
    public Guid FarmId { get; init; }
    public string FullName { get; init; }
    public string Position { get; init; }
    public string ContractType { get; init; }
    public decimal Salary { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string Comment { get; init; }
}

public record AddEmployeeCommand(AddEmployeeData Data) : IRequest<EmptyBaseResponse>;

public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public AddEmployeeCommandHandler(IUserDataResolver userDataResolver, IEmployeeRepository employeeRepository,
        IFarmRepository farmRepository)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _farmRepository = farmRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), cancellationToken);

        var existingEmployee =
            await _employeeRepository.FirstOrDefaultAsync(new GetEmployeeByFullNameSpec(request.Data.FullName),
                cancellationToken);
        if (existingEmployee is not null)
        {
            throw new Exception($"Pracownik '{request.Data.FullName}' już istnieje w systemie.");
        }

        var newEmployee = EmployeeEntity.CreateNew(
            farm.Id,
            request.Data.FullName,
            request.Data.Position,
            request.Data.ContractType,
            request.Data.Salary,
            request.Data.StartDate,
            request.Data.EndDate,
            request.Data.Comment,
            userId
        );

        await _employeeRepository.AddAsync(newEmployee, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
{
    public AddEmployeeCommandValidator()
    {
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.FarmId).NotEmpty();
        RuleFor(x => x.Data.FullName)
            .NotEmpty()
            .MinimumLength(3)
            .Must(name => name.Contains(' ')).WithMessage("Należy podać imię i nazwisko.");
        RuleFor(x => x.Data.Position).NotEmpty();
        RuleFor(x => x.Data.ContractType).NotEmpty();
        RuleFor(x => x.Data.Salary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.StartDate).NotEmpty();
    }
}

public sealed class GetEmployeeByFullNameSpec : BaseSpecification<EmployeeEntity>,
    ISingleResultSpecification<EmployeeEntity>
{
    public GetEmployeeByFullNameSpec(string fullName)
    {
        EnsureExists();
        Query.Where(t => EF.Functions.ILike(t.FullName, $"%{fullName}%"));
    }
}