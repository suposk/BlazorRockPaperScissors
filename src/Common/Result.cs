namespace MudBlazorRokPaperScissors.Common;

public interface IResult
{
    string? ErrorMessage { get; }
    bool IsSuccess { get; init; }
    Error? Error { get; init; }
}
public interface IResult<out T> : IResult
{
    T? Value { get; }
}


public class Result : IResult
{
    internal Result() { }

    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException($"must provide Error.None");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException($"must provide {nameof(error)}");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }

    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage => Error?.Message;
    public static Result Success() => new(true, Error.None);
    public static ValueTask<Result> SuccessAsync() => ValueTask.FromResult(new Result(true, Error.None));    
    public static Result Failure(string? errorMessage) => new(false, new Error(errorMessage));
    public static ValueTask<Result> FailureAsync(string? errorMessage) => ValueTask.FromResult(new Result(false, new Error(errorMessage)));    
}

public class ResultCom<T> : Result, IResult<T>
{
    public T? Value { get; set; }

    public static new ResultCom<T> Failure(string? errorMessage) => new ResultCom<T> { IsSuccess = false, Error = new Error(errorMessage) };
    public static new async ValueTask<ResultCom<T>> FailureAsync(string? errorMessage) => await ValueTask.FromResult(Failure(errorMessage));
    public static ResultCom<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static async ValueTask<ResultCom<T>> SuccessAsync(T value) => await ValueTask.FromResult(Success(value));
}


public class Error
{
    public string? Message { get; init; }    

    public Error(string? message)
    {
        Message = message ??= string.Empty;                
    }

    public static readonly Error None = new(string.Empty);
}
