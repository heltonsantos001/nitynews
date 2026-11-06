namespace nitynews.Exceptions;

/// <summary>
/// Representa uma violação de regra de negócio da aplicação (HTTP 400).
/// </summary>
public class BusinessException : AppException
{
    public BusinessException(string message) : base(message, 400)
    {
    }
}
