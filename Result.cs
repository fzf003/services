public record Result
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    protected Result(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }
}

public record Result<T> : Result
{
    public T Value { get; }

    private Result(T value, bool isSuccess, string errorMessage)
        : base(isSuccess, errorMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(default, false, errorMessage);
    }
}
