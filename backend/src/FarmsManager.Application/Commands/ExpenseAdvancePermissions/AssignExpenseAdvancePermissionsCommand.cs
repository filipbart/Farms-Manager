using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Specifications;
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
    public Guid ExpenseAdvanceRegistryId { get; init; }
    public List<ExpenseAdvancePermissionType> PermissionTypes { get; init; }
}

public record AssignExpenseAdvancePermissionsCommand(AssignExpenseAdvancePermissionsData Data)
    : IRequest<BaseResponse<List<ExpenseAdvancePermissionDto>>>;

public class AssignExpenseAdvancePermissionsCommandHandler : IRequestHandler<AssignExpenseAdvancePermissionsCommand,
    BaseResponse<List<ExpenseAdvancePermissionDto>>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IExpenseAdvanceRegistryRepository _registryRepository;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;

    public AssignExpenseAdvancePermissionsCommandHandler(
        IUserDataResolver userDataResolver,
        IUserRepository userRepository,
        IExpenseAdvanceRegistryRepository registryRepository,
        IExpenseAdvancePermissionRepository permissionRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _registryRepository = registryRepository;
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

        // Sprawdź czy ewidencja istnieje
        var registry = await _registryRepository.GetByIdAsync(request.Data.ExpenseAdvanceRegistryId, cancellationToken);
        if (registry == null)
        {
            throw DomainException.RecordNotFound("Ewidencja zaliczek nie istnieje.");
        }

        if (!registry.IsActive)
        {
            throw new Exception("Ewidencja zaliczek jest nieaktywna.");
        }

        // Pobierz istniejące uprawnienia
        var existingPermissions = await _permissionRepository.ListAsync(
            new GetUserRegistryPermissionsSpec(request.Data.UserId, request.Data.ExpenseAdvanceRegistryId),
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
                request.Data.ExpenseAdvanceRegistryId,
                permissionType,
                currentUserId);

            await _permissionRepository.AddAsync(permission, cancellationToken);
            createdPermissions.Add(permission);
        }

        var permissionDtos = createdPermissions.Select(p => new ExpenseAdvancePermissionDto
        {
            Id = p.Id,
            UserId = p.UserId,
            ExpenseAdvanceRegistryId = p.ExpenseAdvanceRegistryId,
            ExpenseAdvanceRegistryName = registry.Name,
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

        RuleFor(x => x.Data.ExpenseAdvanceRegistryId)
            .NotEmpty()
            .WithMessage("ExpenseAdvanceRegistryId jest wymagane.");

        RuleFor(x => x.Data.PermissionTypes)
            .NotEmpty()
            .WithMessage("Lista uprawnień nie może być pusta.");

        RuleFor(x => x.Data.PermissionTypes)
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Lista uprawnień zawiera duplikaty.");
    }
}

public sealed class GetUserRegistryPermissionsSpec : BaseSpecification<ExpenseAdvancePermissionEntity>
{
    public GetUserRegistryPermissionsSpec(Guid userId, Guid registryId)
    {
        EnsureExists();
        Query.Where(p => p.UserId == userId && p.ExpenseAdvanceRegistryId == registryId);
    }
}
