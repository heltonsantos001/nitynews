using nitynews.Model;

namespace nitynews.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Guid> CadastrarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task<Usuario?> BuscarUsuario(Guid idUsuario, CancellationToken cancellationToken = default);
    Task<List<Usuario>> BuscarTodosUsuarios(CancellationToken cancellationToken = default);
    Task<Usuario?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, Guid? ignorarId = null, CancellationToken cancellationToken = default);
    Task AtualizarUsuarioAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task DeletarUsuario(CancellationToken cancellationToken = default);
}
