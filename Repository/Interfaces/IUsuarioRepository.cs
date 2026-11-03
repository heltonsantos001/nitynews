using nitynews.Model;

namespace nitynews.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Guid> CadastrarUsuarioAsync(Usuario usuario);
    Task<Usuario?> BuscarUsuario(Guid idUsuario);
}
