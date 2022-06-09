namespace GQL.Workshop.App.Filters;

public class AppErrorFilter : IErrorFilter
{
    public IError OnError(IError error) => error.Exception switch
    {
        null => error,
        AppError appError => error.WithMessage(appError.Message).WithCode(appError.ErrorCode),
        _ => error.WithMessage(error.Exception.Message)
    };
}