using nitynews.Dtos;
using nitynews.Model;
using nitynews.Repository.Interfaces;
using nitynews.Services.Interfaces;

namespace nitynews.Services;

public class UsuarioService : IUsuarioService 
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Guid> CadastrarAsync(UsuarioDto dto)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Ativo = dto.Ativo ?? true
        };

        return await _usuarioRepository.CadastrarUsuarioAsync(usuario);
    }

    public async Task<UsuarioDto?> BuscarUsuario(Guid idUsuario)
    {
        var usuario = await _usuarioRepository.BuscarUsuario(idUsuario);
        if (usuario == null)
            return null;

        return new UsuarioDto
        {
            Nome = usuario.Nome,
            Email = usuario.Email,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro,
            EmailVerificado = usuario.EmailVerificado
        };
    }
}
