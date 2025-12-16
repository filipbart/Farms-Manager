namespace FarmsManager.Application.Models.ExpenseAdvanceColumnSettings;

public record ExpenseAdvanceColumnSettingsDto
{
    public Guid UserId { get; init; }
    public List<string> VisibleColumns { get; init; } = new();
}

public record AvailableColumnDto
{
    public string Key { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record GetExpenseAdvanceColumnSettingsResponse
{
    public List<string> VisibleColumns { get; init; } = new();
    public List<AvailableColumnDto> AvailableColumns { get; init; } = new();
}
