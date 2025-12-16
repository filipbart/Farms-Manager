using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Constants;
using FarmsManager.Application.Models.ExpenseAdvanceColumnSettings;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ExpenseAdvanceColumnSettings;

public record GetUserExpenseAdvanceColumnSettingsQuery(Guid UserId) 
    : IRequest<BaseResponse<GetExpenseAdvanceColumnSettingsResponse>>;

public class GetUserExpenseAdvanceColumnSettingsQueryHandler 
    : IRequestHandler<GetUserExpenseAdvanceColumnSettingsQuery, BaseResponse<GetExpenseAdvanceColumnSettingsResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserExpenseAdvanceColumnSettingsRepository _columnSettingsRepository;

    public GetUserExpenseAdvanceColumnSettingsQueryHandler(
        IUserRepository userRepository,
        IUserExpenseAdvanceColumnSettingsRepository columnSettingsRepository)
    {
        _userRepository = userRepository;
        _columnSettingsRepository = columnSettingsRepository;
    }

    public async Task<BaseResponse<GetExpenseAdvanceColumnSettingsResponse>> Handle(
        GetUserExpenseAdvanceColumnSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw DomainException.RecordNotFound("Użytkownik nie istnieje.");
        }

        var settings = await _columnSettingsRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        var visibleColumns = settings?.GetVisibleColumnsList() 
            ?? ExpenseAdvanceColumns.DefaultColumns.ToList();

        var availableColumns = ExpenseAdvanceColumns.ColumnDescriptions
            .Select(kvp => new AvailableColumnDto
            {
                Key = kvp.Key,
                Description = kvp.Value
            })
            .ToList();

        return BaseResponse.CreateResponse(new GetExpenseAdvanceColumnSettingsResponse
        {
            VisibleColumns = visibleColumns,
            AvailableColumns = availableColumns
        });
    }
}
