namespace nitynews.Exceptions;

/// <summary>
/// Representa um conflito de dados (ex: duplicação de chave única, e-mail já existente) (HTTP 409).
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, 409)
    {
    }
}
