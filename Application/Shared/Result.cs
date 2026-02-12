using Application.Common.Errors;

namespace Application.Shared;

public class Result<T>
{

    private Result(T value)
    {
        Value = value;
    }

    private Result(List<Error> errorsList)
    {
        ErrorsList = errorsList;
    }
    public T? Value { get; }
    public bool IsSuccess => !ErrorsList.Any();
    public List<Error> ErrorsList { get; set; } = new();

    public static Result<T> Success(T value)
        => new Result<T>(value);
    public static Result<T> Failure(Error error)
        => new Result<T>(new List<Error> { error });
    public static Result<T> Failure(List<Error> errors)
         => new Result<T>(errors);
    public TResult Map<TResult>(Func<T, TResult> onSuccess, Func<IEnumerable<Error>, TResult> onFailure)
     => IsSuccess ? onSuccess(Value!) : onFailure(ErrorsList!);

}
