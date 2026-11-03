using nitynews.Dtos;

namespace nitynews.Services.Interfaces;

public interface IUsuarioService
{
	Task<Guid> CadastrarAsync(UsuarioDto dto);
	Task<UsuarioDto?> BuscarUsuario(Guid idUsuario); 
}

