namespace nitynews.Exceptions;

/// <summary>
/// Representa quando um recurso solicitado não foi encontrado no sistema (HTTP 404).
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message = "Recurso não encontrado.")
        : base(message, 404)
    {
    }
}
