using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

/// <summary>
/// Encja przechowująca ustawienia widoczności kolumn w ewidencji zaliczek dla użytkownika
/// </summary>
public class UserExpenseAdvanceColumnSettingsEntity : Entity
{
    public Guid UserId { get; protected internal set; }
    
    /// <summary>
    /// Lista widocznych kolumn przechowywana jako string rozdzielony przecinkami
    /// </summary>
    public string VisibleColumns { get; protected internal set; }

    public virtual UserEntity User { get; protected internal set; }

    protected UserExpenseAdvanceColumnSettingsEntity()
    {
    }

    public static UserExpenseAdvanceColumnSettingsEntity CreateNew(
        Guid userId,
        IEnumerable<string> visibleColumns,
        Guid? creatorId = null)
    {
        return new UserExpenseAdvanceColumnSettingsEntity
        {
            UserId = userId,
            VisibleColumns = string.Join(",", visibleColumns),
            CreatedBy = creatorId
        };
    }

    public void UpdateVisibleColumns(IEnumerable<string> visibleColumns, Guid? modifierId = null)
    {
        VisibleColumns = string.Join(",", visibleColumns);
        SetModified(modifierId);
    }

    public List<string> GetVisibleColumnsList()
    {
        if (string.IsNullOrWhiteSpace(VisibleColumns))
            return [];
        
        return VisibleColumns.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
