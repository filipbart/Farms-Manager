using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ExpenseAdvancePermissions;

public record AssignExpenseAdvancePermissionsData
{
    public Guid UserId { get; init; }
    public Guid ExpenseAdvanceId { get; init; }  // To jest EmployeeId
    public List<ExpenseAdvancePermissionType> PermissionTypes { get; init; }
}

public record AssignExpenseAdvancePermissionsCommand(AssignExpenseAdvancePermissionsData Data)
    : IRequest<BaseResponse<List<ExpenseAdvancePermissionDto>>>;

public class AssignExpenseAdvancePermissionsCommandHandler : IRequestHandler<AssignExpenseAdvancePermissionsCommand,
    BaseResponse<List<ExpenseAdvancePermissionDto>>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;

    public AssignExpenseAdvancePermissionsCommandHandler(
        IUserDataResolver userDataResolver,
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IExpenseAdvancePermissionRepository permissionRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<List<ExpenseAdvancePermissionDto>>> Handle(
        AssignExpenseAdvancePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        // Sprawdź czy użytkownik istnieje
        var user = await _userRepository.GetByIdAsync(request.Data.UserId, cancellationToken);
        if (user == null)
        {
            throw DomainException.RecordNotFound("Użytkownik nie istnieje.");
        }

        // Sprawdź czy pracownik istnieje
        var employee = await _employeeRepository.GetByIdAsync(request.Data.ExpenseAdvanceId, cancellationToken);
        if (employee == null)
        {
            throw DomainException.RecordNotFound("Pracownik nie istnieje.");
        }

        // Pobierz istniejące uprawnienia
        var existingPermissions = await _permissionRepository.ListAsync(
            new GetUserEmployeePermissionsSpec(request.Data.UserId, request.Data.ExpenseAdvanceId),
            cancellationToken);

        var createdPermissions = new List<ExpenseAdvancePermissionEntity>();

        foreach (var permissionType in request.Data.PermissionTypes)
        {
            // Sprawdź czy uprawnienie już istnieje
            if (existingPermissions.Any(p => p.PermissionType == permissionType))
            {
                throw new Exception($"Użytkownik już posiada uprawnienie {permissionType} dla tej ewidencji.");
            }

            var permission = ExpenseAdvancePermissionEntity.CreateNew(
                request.Data.UserId,
                request.Data.ExpenseAdvanceId,
                permissionType,
                currentUserId);

            await _permissionRepository.AddAsync(permission, cancellationToken);
            createdPermissions.Add(permission);
        }

        var permissionDtos = createdPermissions.Select(p => new ExpenseAdvancePermissionDto
        {
            Id = p.Id,
            UserId = p.UserId,
            ExpenseAdvanceId = p.EmployeeId,
            EmployeeName = employee.FullName,
            PermissionType = p.PermissionType,
            DateCreatedUtc = p.DateCreatedUtc,
            DateModifiedUtc = p.DateModifiedUtc
        }).ToList();

        return BaseResponse.CreateResponse(permissionDtos);
    }
}

public class AssignExpenseAdvancePermissionsCommandValidator : AbstractValidator<AssignExpenseAdvancePermissionsCommand>
{
    public AssignExpenseAdvancePermissionsCommandValidator()
    {
        RuleFor(x => x.Data.UserId)
            .NotEmpty()
            .WithMessage("UserId jest wymagane.");

        RuleFor(x => x.Data.ExpenseAdvanceId)
            .NotEmpty()
            .WithMessage("ExpenseAdvanceId jest wymagane.");

        RuleFor(x => x.Data.PermissionTypes)
            .NotEmpty()
            .WithMessage("Lista uprawnień nie może być pusta.");

        RuleFor(x => x.Data.PermissionTypes)
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Lista uprawnień zawiera duplikaty.");
    }
}

public sealed class GetUserEmployeePermissionsSpec : BaseSpecification<ExpenseAdvancePermissionEntity>
{
    public GetUserEmployeePermissionsSpec(Guid userId, Guid employeeId)
    {
        EnsureExists();
        Query.Where(p => p.UserId == userId && p.EmployeeId == employeeId);
    }
}
