using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ExpenseAdvancePermissions;

public record GetUserExpenseAdvancePermissionsQuery(Guid UserId) 
    : IRequest<BaseResponse<UserExpenseAdvancePermissionsDto>>;

public class GetUserExpenseAdvancePermissionsQueryHandler : IRequestHandler<GetUserExpenseAdvancePermissionsQuery,
    BaseResponse<UserExpenseAdvancePermissionsDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseAdvancePermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetUserExpenseAdvancePermissionsQueryHandler(
        IUserRepository userRepository,
        IExpenseAdvancePermissionRepository permissionRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UserExpenseAdvancePermissionsDto>> Handle(
        GetUserExpenseAdvancePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw DomainException.RecordNotFound("Użytkownik nie istnieje.");
        }

        var permissions = await _permissionRepository.ListAsync(
            new GetUserExpenseAdvancePermissionsSpec(request.UserId),
            cancellationToken);

        var permissionDtos = _mapper.Map<List<ExpenseAdvancePermissionDto>>(permissions);

        return BaseResponse.CreateResponse(new UserExpenseAdvancePermissionsDto
        {
            UserId = request.UserId,
            Permissions = permissionDtos
        });
    }
}

public sealed class GetUserExpenseAdvancePermissionsSpec : BaseSpecification<ExpenseAdvancePermissionEntity>
{
    public GetUserExpenseAdvancePermissionsSpec(Guid userId)
    {
        EnsureExists();
        Query.Where(p => p.UserId == userId);
        Query.Include(p => p.Employee);
        Query.OrderBy(p => p.Employee.FullName);
    }
}
