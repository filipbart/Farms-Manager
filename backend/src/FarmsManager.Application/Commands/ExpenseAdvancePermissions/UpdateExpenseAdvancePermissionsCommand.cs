using FarmsManager.Application.Commands.ExpenseAdvancePermissions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ExpenseAdvancePermissions;

public record UpdateExpenseAdvancePermissionsData
{
    public Guid PermissionId { get; init; }
    public List<ExpenseAdvancePermissionType> PermissionTypes { get; init; }
}

public record UpdateExpenseAdvancePermissionsCommand(UpdateExpenseAdvancePermissionsData Data)
    : IRequest<BaseResponse<List<ExpenseAdvancePermissionDto>>>;

public class UpdateExpenseAdvancePermissionsCommandHandler : IRequestHandler<UpdateExpenseAdvancePermissionsCommand,
    BaseResponse<List<ExpenseAdvancePermissionDto>>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public UpdateExpenseAdvancePermissionsCommandHandler(
        IUserDataResolver userDataResolver,
        IExpenseAdvancePermissionRepository permissionRepository,
        IEmployeeRepository employeeRepository)
    {
        _userDataResolver = userDataResolver;
        _permissionRepository = permissionRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<BaseResponse<List<ExpenseAdvancePermissionDto>>> Handle(
        UpdateExpenseAdvancePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        // Znajdź istniejące uprawnienie
        var existingPermission = await _permissionRepository.GetByIdAsync(request.Data.PermissionId, cancellationToken);
        if (existingPermission == null)
        {
            throw DomainException.RecordNotFound("Uprawnienie nie istnieje.");
        }

        var userId = existingPermission.UserId;
        var employeeId = existingPermission.EmployeeId;

        // Pobierz pracownika
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw DomainException.RecordNotFound("Pracownik nie istnieje.");
        }

        // Usuń wszystkie stare uprawnienia dla tej kombinacji użytkownik-pracownik
        var allUserPermissions = await _permissionRepository.ListAsync(
            new GetUserEmployeePermissionsSpec(userId, employeeId),
            cancellationToken);

        foreach (var permission in allUserPermissions)
        {
            permission.Delete(currentUserId);
        }

        // Utwórz nowe uprawnienia
        var newPermissions = new List<ExpenseAdvancePermissionEntity>();
        foreach (var permissionType in request.Data.PermissionTypes)
        {
            var permission = ExpenseAdvancePermissionEntity.CreateNew(
                userId,
                employeeId,
                permissionType,
                currentUserId);

            await _permissionRepository.AddAsync(permission, cancellationToken);
            newPermissions.Add(permission);
        }

        var permissionDtos = newPermissions.Select(p => new ExpenseAdvancePermissionDto
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

public class UpdateExpenseAdvancePermissionsCommandValidator : AbstractValidator<UpdateExpenseAdvancePermissionsCommand>
{
    public UpdateExpenseAdvancePermissionsCommandValidator()
    {
        RuleFor(x => x.Data.PermissionId)
            .NotEmpty()
            .WithMessage("PermissionId jest wymagane.");

        RuleFor(x => x.Data.PermissionTypes)
            .NotEmpty()
            .WithMessage("Lista uprawnień nie może być pusta.");

        RuleFor(x => x.Data.PermissionTypes)
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Lista uprawnień zawiera duplikaty.");
    }
}
