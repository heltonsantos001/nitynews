using nitynews.Data;
using nitynews.Model;
using nitynews.Repository.Interfaces;

namespace nitynews.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CadastrarUsuarioAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
        return usuario.Id;
    }

    public async Task<Usuario?> BuscarUsuario(Guid idUsuario)
    {
        return await _context.Usuarios.FindAsync(idUsuario);
    }
}
