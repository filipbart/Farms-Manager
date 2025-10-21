namespace FarmsManager.Application.Common;

public record PaginationParams
{
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public bool? ShowDeleted { get; set; }

    public int Skip => Page * PageSize;
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