using System.ComponentModel;
using System.Reflection;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Permissions;
using MediatR;

// Namespace z Twoją klasą AppPermissions

namespace FarmsManager.Application.Queries.Users;

public record PermissionModel
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string Group { get; init; }
}

public record GetPermissionsQueryResponse
{
    public List<PermissionModel> Items { get; init; } = [];
}

public record GetPermissionsQuery : IRequest<BaseResponse<GetPermissionsQueryResponse>>;

public class
    GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, BaseResponse<GetPermissionsQueryResponse>>
{
    public Task<BaseResponse<GetPermissionsQueryResponse>> Handle(GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = typeof(AppPermissions).GetAllPermissions();

        var response = new GetPermissionsQueryResponse { Items = permissions };
        return Task.FromResult(BaseResponse.CreateResponse(response));
    }
}