namespace Ipco.Domain;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (isSuccess && error != Error.None)
            throw new ArgumentException("A successful result cannot contain an error.", nameof(error));

        if (!isSuccess && error == Error.None)
            throw new ArgumentException("A failed result must contain an error.", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result(false, error);
    }
}

public sealed class Result<T> : Result
{
    private readonly T? _value;
    private Result(T value) : base(true, Error.None) => _value = value;
    private Result(Error error) : base(false, error) { }

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("A failed result has no value.");

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return IsSuccess ? Result<TOut>.Success(mapper(Value)) : Result<TOut>.Failure(Error);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static new Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(error);
    }
}
