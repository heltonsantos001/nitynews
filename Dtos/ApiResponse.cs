namespace nitynews.Dtos;

/// <summary>
/// Estrutura genérica para padronização de respostas da API.
/// Funciona com qualquer classe ou tipo de dado (ex: ApiResponse<UsuarioDto>, ApiResponse<Guid>, ApiResponse<List<UsuarioDto>>).
/// </summary>
/// <typeparam name="T">Tipo do dado retornado no campo Data.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi realizada com sucesso (true) ou se houve falha (false).
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Mensagem explicativa de sucesso ou descrição do erro.
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Objeto ou lista retornada pela operação. Pode ser nulo quando não houver retorno ou em caso de falha.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Lista de mensagens de erros ou validações detalhadas (quando houver).
    /// </summary>
    public List<string>? Erros { get; set; }

    /// <summary>
    /// Data e hora da resposta em formato UTC.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ApiResponse()
    {
    }

    public ApiResponse(bool sucesso, string mensagem, T? data = default, List<string>? erros = null)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        Data = data;
        Erros = erros;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Atalho para retorno de sucesso com payload de dados.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string mensagem = "Operação realizada com sucesso.")
    {
        return new ApiResponse<T>(true, mensagem, data);
    }

    /// <summary>
    /// Atalho para retorno de falha com mensagem explicativa e lista opcional de erros.
    /// </summary>
    public static ApiResponse<T> Falha(string mensagem, List<string>? erros = null, T? data = default)
    {
        return new ApiResponse<T>(false, mensagem, data, erros);
    }

    /// <summary>
    /// Atalho para retorno de falha com uma única mensagem de erro de detalhe.
    /// </summary>
    public static ApiResponse<T> Falha(string mensagem, string erro, T? data = default)
    {
        return new ApiResponse<T>(false, mensagem, data, new List<string> { erro });
    }
}

/// <summary>
/// Versão não-genérica para respostas que não precisam transportar dados (apenas mensagem, erros e status booleano).
/// </summary>
public class ApiResponse : ApiResponse<object?>
{
    public ApiResponse()
    {
    }

    public ApiResponse(bool sucesso, string mensagem, object? data = null, List<string>? erros = null)
        : base(sucesso, mensagem, data, erros)
    {
    }

    public static ApiResponse Ok(string mensagem = "Operação realizada com sucesso.")
    {
        return new ApiResponse(true, mensagem);
    }

    public static ApiResponse Falha(string mensagem, List<string>? erros = null)
    {
        return new ApiResponse(false, mensagem, null, erros);
    }

    public static ApiResponse Falha(string mensagem, string erro)
    {
        return new ApiResponse(false, mensagem, null, new List<string> { erro });
    }
}
