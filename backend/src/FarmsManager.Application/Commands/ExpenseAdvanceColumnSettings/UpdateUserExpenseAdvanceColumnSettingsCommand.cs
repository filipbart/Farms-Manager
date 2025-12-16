using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Constants;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ExpenseAdvanceColumnSettings;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.ExpenseAdvanceColumnSettings;

public record UpdateUserExpenseAdvanceColumnSettingsData
{
    public Guid UserId { get; init; }
    public List<string> VisibleColumns { get; init; } = new();
}

public record UpdateUserExpenseAdvanceColumnSettingsCommand(UpdateUserExpenseAdvanceColumnSettingsData Data) 
    : IRequest<BaseResponse<ExpenseAdvanceColumnSettingsDto>>;

public class UpdateUserExpenseAdvanceColumnSettingsCommandHandler 
    : IRequestHandler<UpdateUserExpenseAdvanceColumnSettingsCommand, BaseResponse<ExpenseAdvanceColumnSettingsDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserExpenseAdvanceColumnSettingsRepository _columnSettingsRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateUserExpenseAdvanceColumnSettingsCommandHandler(
        IUserRepository userRepository,
        IUserExpenseAdvanceColumnSettingsRepository columnSettingsRepository,
        IUserDataResolver userDataResolver)
    {
        _userRepository = userRepository;
        _columnSettingsRepository = columnSettingsRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<ExpenseAdvanceColumnSettingsDto>> Handle(
        UpdateUserExpenseAdvanceColumnSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Data.UserId, cancellationToken);
        if (user == null)
        {
            throw DomainException.RecordNotFound("Użytkownik nie istnieje.");
        }

        // Walidacja kolumn - sprawdź czy wszystkie podane kolumny są prawidłowe
        var validColumns = request.Data.VisibleColumns
            .Where(c => ExpenseAdvanceColumns.AllColumns.Contains(c))
            .Distinct()
            .ToList();

        if (validColumns.Count == 0)
        {
            throw DomainException.BadRequest("Musisz wybrać co najmniej jedną kolumnę.");
        }

        var existingSettings = await _columnSettingsRepository.GetByUserIdAsync(request.Data.UserId, cancellationToken);
        var currentUserId = _userDataResolver.GetUserId();

        if (existingSettings != null)
        {
            existingSettings.UpdateVisibleColumns(validColumns, currentUserId);
            _columnSettingsRepository.Update(existingSettings);
        }
        else
        {
            var newSettings = UserExpenseAdvanceColumnSettingsEntity.CreateNew(
                request.Data.UserId,
                validColumns,
                currentUserId);
            await _columnSettingsRepository.AddAsync(newSettings, cancellationToken);
        }

        await _columnSettingsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(new ExpenseAdvanceColumnSettingsDto
        {
            UserId = request.Data.UserId,
            VisibleColumns = validColumns
        });
    }
}
