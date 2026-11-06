using nitynews.Dtos;

namespace nitynews.Services.Interfaces;

public interface IUsuarioService
{
    Task<Guid> CadastrarAsync(UsuarioDto dto, CancellationToken cancellationToken = default);
    Task<UsuarioDto?> BuscarUsuario(Guid idUsuario, CancellationToken cancellationToken = default);
    Task<List<UsuarioDto>> BuscarTodosUsuarios(CancellationToken cancellationToken = default);
    Task<UsuarioDto?> DeletarUsuario(Guid id, CancellationToken cancellationToken = default);
}
