using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using nitynews.Dtos;
using nitynews.Exceptions;

namespace nitynews.Middlewares;

/// <summary>
/// Middleware global para captura e tratamento centralizado de exceções.
/// Garante que nenhuma exceção não tratada quebre a aplicação ou vaze dados sensíveis.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning(ex, "A resposta já havia sido iniciada quando ocorreu uma exceção.");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string mensagem;
        List<string>? erros = null;

        switch (exception)
        {
            case AppException appEx:
                statusCode = appEx.StatusCode;
                mensagem = appEx.Message;
                _logger.LogWarning(exception, "Exceção de negócio tratada ({StatusCode}): {Message}", statusCode, mensagem);
                break;

            case BadHttpRequestException badHttpEx:
                statusCode = StatusCodes.Status400BadRequest;
                mensagem = "Requisição malformada ou dados inválidos.";
                erros = new List<string> { badHttpEx.Message };
                _logger.LogWarning(exception, "Requisição HTTP malformada: {Message}", badHttpEx.Message);
                break;

            case ArgumentNullException argNullEx:
                statusCode = StatusCodes.Status400BadRequest;
                mensagem = "Parâmetro obrigatório não fornecido.";
                erros = new List<string> { argNullEx.Message };
                _logger.LogWarning(exception, "Argumento nulo: {Message}", argNullEx.Message);
                break;

            case ArgumentException argEx:
                statusCode = StatusCodes.Status400BadRequest;
                mensagem = "Parâmetro inválido fornecido na requisição.";
                erros = new List<string> { argEx.Message };
                _logger.LogWarning(exception, "Argumento inválido: {Message}", argEx.Message);
                break;

            case DbUpdateException dbEx:
                statusCode = StatusCodes.Status409Conflict;
                mensagem = "Conflito de dados no banco de dados. Verifique se os dados informados (como e-mail) já não existem.";
                erros = new List<string> { "Falha ao persistir alterações devido a restrições de unicidade ou integridade." };
                _logger.LogError(dbEx, "Erro de integridade de banco de dados na rota {Path}", context.Request.Path);
                break;

            case OperationCanceledException:
                statusCode = 499; // Client Closed Request
                mensagem = "A requisição foi cancelada antes da conclusão.";
                _logger.LogInformation("Requisição cancelada na rota {Path}", context.Request.Path);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                mensagem = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.";
                _logger.LogError(exception, "Exceção não tratada capturada pelo middleware na rota {Method} {Path}", context.Request.Method, context.Request.Path);

                if (_environment.IsDevelopment())
                {
                    erros = new List<string>
                    {
                        exception.Message,
                        exception.StackTrace ?? string.Empty
                    };
                }
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse.Falha(mensagem, erros);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}
