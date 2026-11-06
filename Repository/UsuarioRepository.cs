using Microsoft.EntityFrameworkCore;
using nitynews.Data;
using nitynews.Model;
using nitynews.Repository.Interfaces;

namespace nitynews.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(AppDbContext context, ILogger<UsuarioRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> CadastrarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        try
        {
            await _context.Usuarios.AddAsync(usuario, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Usuário com ID {Id} e e-mail {Email} inserido no banco com sucesso.", usuario.Id, usuario.Email);
            return usuario.Id;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de atualização ao cadastrar usuário com e-mail: {Email}", usuario.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao cadastrar usuário com e-mail: {Email}", usuario.Email);
            throw;
        }
    }

    public async Task<Usuario?> BuscarUsuario(Guid idUsuario, CancellationToken cancellationToken = default)
    {
        if (idUsuario == Guid.Empty)
        {
            _logger.LogWarning("Tentativa de buscar usuário com Guid vazio.");
            return null;
        }

        try
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário pelo ID {Id}", idUsuario);
            throw;
        }
    }

    public async Task<List<Usuario>> BuscarTodosUsuarios(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Usuarios
                .AsNoTracking()
                .OrderByDescending(u => u.DataCadastro)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar todos os usuários.");
            throw;
        }
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        try
        {
            string emailFormatado = email.Trim().ToLowerInvariant();
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailFormatado, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário pelo e-mail {Email}", email);
            throw;
        }
    }

    public async Task<bool> ExisteEmailAsync(string email, Guid? ignorarId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            string emailFormatado = email.Trim().ToLowerInvariant();
            var query = _context.Usuarios.AsNoTracking().Where(u => u.Email.ToLower() == emailFormatado);

            if (ignorarId.HasValue && ignorarId.Value != Guid.Empty)
            {
                query = query.Where(u => u.Id != ignorarId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência de e-mail {Email}", email);
            throw;
        }
    }

    public async Task AtualizarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        try
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Usuário com ID {Id} atualizado com sucesso.", usuario.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de atualização ao atualizar usuário com ID {Id}", usuario.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar usuário com ID {Id}", usuario.Id);
            throw;
        }
    }

    public async Task DeletarUsuario(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao persistir exclusão/desativação de usuário.");
            throw;
        }
    }
}
