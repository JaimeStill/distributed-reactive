namespace Topics.Core;

public enum ExceptionType
{
    Validation,
    Authorization
}

public class AppException : Exception
{
    public ExceptionType ExceptionType { get; set; }

    public AppException(string message, ExceptionType exceptionType = ExceptionType.Validation) : base(message)
    {
        ExceptionType = exceptionType;
    }
}