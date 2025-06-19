namespace FarmsManager.Application.Common.Responses;

public class EmptyBaseResponse : BaseResponse<object>
{
    public EmptyBaseResponse()
    {
    }
}

public class EmptyBaseResponse<T> : BaseResponse<T>;