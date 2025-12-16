using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Constants;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ExpenseAdvanceColumnSettings;

public record GetCurrentUserExpenseAdvanceColumnSettingsResponse
{
    public List<string> VisibleColumns { get; init; } = new();
    public bool IsAdmin { get; init; }
    public bool HasAllPermissions { get; init; }
}

public record GetCurrentUserExpenseAdvanceColumnSettingsQuery 
    : IRequest<BaseResponse<GetCurrentUserExpenseAdvanceColumnSettingsResponse>>;

public class GetCurrentUserExpenseAdvanceColumnSettingsQueryHandler 
    : IRequestHandler<GetCurrentUserExpenseAdvanceColumnSettingsQuery, BaseResponse<GetCurrentUserExpenseAdvanceColumnSettingsResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserExpenseAdvanceColumnSettingsRepository _columnSettingsRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetCurrentUserExpenseAdvanceColumnSettingsQueryHandler(
        IUserRepository userRepository,
        IUserExpenseAdvanceColumnSettingsRepository columnSettingsRepository,
        IUserDataResolver userDataResolver)
    {
        _userRepository = userRepository;
        _columnSettingsRepository = columnSettingsRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetCurrentUserExpenseAdvanceColumnSettingsResponse>> Handle(
        GetCurrentUserExpenseAdvanceColumnSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userDataResolver.GetUserId();
        if (currentUserId == null)
        {
            return BaseResponse.CreateResponse(new GetCurrentUserExpenseAdvanceColumnSettingsResponse
            {
                VisibleColumns = ExpenseAdvanceColumns.DefaultColumns.ToList(),
                IsAdmin = false,
                HasAllPermissions = false
            });
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return BaseResponse.CreateResponse(new GetCurrentUserExpenseAdvanceColumnSettingsResponse
            {
                VisibleColumns = ExpenseAdvanceColumns.DefaultColumns.ToList(),
                IsAdmin = false,
                HasAllPermissions = false
            });
        }

        // Admin lub osoba ze wszystkimi uprawnieniami widzi wszystkie kolumny
        if (user.IsAdmin)
        {
            return BaseResponse.CreateResponse(new GetCurrentUserExpenseAdvanceColumnSettingsResponse
            {
                VisibleColumns = ExpenseAdvanceColumns.AllColumns.ToList(),
                IsAdmin = true,
                HasAllPermissions = true
            });
        }

        var settings = await _columnSettingsRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);
        
        var visibleColumns = settings?.GetVisibleColumnsList() 
            ?? ExpenseAdvanceColumns.DefaultColumns.ToList();

        return BaseResponse.CreateResponse(new GetCurrentUserExpenseAdvanceColumnSettingsResponse
        {
            VisibleColumns = visibleColumns,
            IsAdmin = false,
            HasAllPermissions = false
        });
    }
}
