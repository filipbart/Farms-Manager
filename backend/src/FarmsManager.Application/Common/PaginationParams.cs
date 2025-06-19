namespace FarmsManager.Application.Common;

public record PaginationParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;
}

public record OrderedPaginationParams<T> : PaginationParams where T : Enum
{
    public T OrderBy { get; set; }
    public bool IsDescending { get; set; } = true;
}

public record OrderedPaginationParams : PaginationParams
{
    public bool IsDescending { get; set; } = true;
}