using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nitynews.Data;
using nitynews.Dtos;
using nitynews.Middlewares;
using nitynews.Repository;
using nitynews.Repository.Interfaces;
using nitynews.Services;
using nitynews.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Controllers com padronização de validação de modelo
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Intercepta erros automáticos de DataAnnotations e formata como ApiResponse padronizada
        options.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err =>
                    string.IsNullOrWhiteSpace(err.ErrorMessage) ? "Valor inválido informado." : err.ErrorMessage))
                .ToList();

            var response = ApiResponse.Falha("Dados inválidos fornecidos na requisição.", erros);
            return new BadRequestObjectResult(response);
        };
    });

// OpenAPI e documentação
builder.Services.AddOpenApi();

// Configuração do Banco de Dados SQLite com connection string validada
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada em appsettings.json.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
);

// Injeção de Dependências das camadas de Repository e Service
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

// Inicialização segura do banco de dados na inicialização
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        
        // Garante que não haja duplicatas antes de criar o índice único
        db.Database.ExecuteSqlRaw("DELETE FROM Usuarios WHERE rowid NOT IN (SELECT MIN(rowid) FROM Usuarios GROUP BY LOWER(Email));");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_Usuarios_Email ON Usuarios (Email);");
        
        logger.LogInformation("Banco de dados SQLite inicializado com índice de e-mail único.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao inicializar o banco de dados SQLite na inicialização.");
    }
}

// Middleware Global de Tratamento de Exceções (captura qualquer erro não tratado na pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
