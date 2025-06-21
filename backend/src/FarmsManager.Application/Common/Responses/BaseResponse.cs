namespace FarmsManager.Application.Common.Responses;

public abstract class BaseResponse
{
    public bool Success => Errors.Count == 0;
    public Dictionary<string, string> Errors { get; set; } = new();

    public void AddError(string key, string message) => Errors.Add(key, message);

    public static BaseResponse<T> CreateResponse<T>(T t) => new(t);
    public static EmptyBaseResponse EmptyResponse => new();
    public static ErrorBaseResponse CreateErrorResponse(Dictionary<string, string> errors) => new() { Errors = errors };
}

public class BaseResponse<T> : BaseResponse
{
    public BaseResponse()
    {
        ResponseTimeUtc = DateTime.UtcNow;
    }

    public BaseResponse(T t)
    {
        ResponseTimeUtc = DateTime.UtcNow;
        ResponseData = t;
    }

    public T ResponseData { get; set; }
    public DateTime ResponseTimeUtc { get; }
}