namespace TaskApi.Services;

public static class ServiceErrorCodes
{
    public const string Validation = "Validation";
    public const string NotFound = "NotFound";
}

public record ServiceError(string Code, string Message);

public class ServiceResult
{
    public bool IsSuccess => Error is null;
    public ServiceError? Error { get; init; }

    public static ServiceResult Success() => new();

    public static ServiceResult Failure(string code, string message) =>
        new() { Error = new ServiceError(code, message) };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data) => new() { Data = data };

    public static new ServiceResult<T> Failure(string code, string message) =>
        new() { Error = new ServiceError(code, message) };
}
