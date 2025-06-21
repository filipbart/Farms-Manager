namespace FarmsManager.Application.Common;

public class PaginationModel<T>
{
    public int TotalRows { get; init; }
    public IEnumerable<T> Items { get; init; }
}