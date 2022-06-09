namespace GQL.Workshop.App;

public abstract class AppError : Exception
{
    public string ErrorCode { get; }

    protected AppError(string message, string errorCode) : base(message) => ErrorCode = errorCode;
}