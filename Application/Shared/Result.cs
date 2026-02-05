using Application.Common.Errors;

namespace Application.Shared;

public class Result<T>
{
    private Result(T value)
    {
        Value = value;
    }
    private Result(Error error)
    {
        Error = error;
    }
    private Result(List<Error> errorsList)
    {
        ErrorsList = errorsList;
    }
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error == null && ErrorsList is null;
    public List<Error> ErrorsList { get; set; }

    public static Result<T> Success(T value)
        => new Result<T>(value);
    public static Result<T> Failure(Error error)
        => new Result<T>(error);
    public static Result<T> Failure(List<Error> errors)
         => new Result<T>(errors);
    public TResult Map<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    public TResult MapList<TResult>(Func<T, TResult> onSuccess, Func<IEnumerable<Error>, TResult> onFailure)
     => IsSuccess ? onSuccess(Value!) : onFailure(ErrorsList!);

}
