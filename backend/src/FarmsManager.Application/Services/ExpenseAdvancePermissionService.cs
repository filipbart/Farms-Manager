using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;

namespace FarmsManager.Application.Services;

public interface IExpenseAdvancePermissionService : IService
{
    Task<bool> HasPermissionAsync(Guid userId, Guid employeeId, ExpenseAdvancePermissionType permissionType, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetAccessibleEmployeeIdsAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class ExpenseAdvancePermissionService : IExpenseAdvancePermissionService
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;

    public ExpenseAdvancePermissionService(
        IUserRepository userRepository,
        IExpenseAdvancePermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId, 
        Guid employeeId, 
        ExpenseAdvancePermissionType permissionType,
        CancellationToken cancellationToken = default)
    {
        // Admin ma dostęp do wszystkiego
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user?.IsAdmin == true)
            return true;

        // Sprawdź czy użytkownik ma uprawnienie
        var permissions = await _permissionRepository.ListAsync(cancellationToken);
        var hasPermission = permissions.Any(p => 
            p.UserId == userId 
            && p.EmployeeId == employeeId 
            && p.PermissionType == permissionType
            && p.DateDeletedUtc == null);

        // Jeśli szuka View, to Edit też daje dostęp
        if (!hasPermission && permissionType == ExpenseAdvancePermissionType.View)
        {
            hasPermission = permissions.Any(p => 
                p.UserId == userId 
                && p.EmployeeId == employeeId 
                && p.PermissionType == ExpenseAdvancePermissionType.Edit
                && p.DateDeletedUtc == null);
        }

        return hasPermission;
    }

    public async Task<List<Guid>> GetAccessibleEmployeeIdsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        // Admin ma dostęp do wszystkich
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user?.IsAdmin == true)
            return new List<Guid>(); // Pusta lista oznacza "wszystkie"

        // Pobierz pracowników do których użytkownik ma dostęp
        var permissions = await _permissionRepository.ListAsync(cancellationToken);
        return permissions
            .Where(p => p.UserId == userId && p.DateDeletedUtc == null)
            .Select(p => p.EmployeeId)
            .Distinct()
            .ToList();
    }
}
