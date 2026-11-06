namespace nitynews.Exceptions;

/// <summary>
/// Exceção base para erros controlados da aplicação.
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
