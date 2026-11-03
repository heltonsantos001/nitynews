using Microsoft.AspNetCore.Mvc;
using nitynews.Dtos;
using nitynews.Services.Interfaces;
namespace nitynews.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Endpoint responsável pelo cadastro de novos usuários.
    /// Rota: POST /api/usuario/cadastrar
    /// </summary>
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar([FromBody] UsuarioDto dto)
    {
        Guid idUsuario = await _usuarioService.CadastrarAsync(dto);
        return Ok(new { mensagem = idUsuario });
    }
    [HttpGet("buscar/{id}")]
    public async Task<IActionResult> BuscarUsuario([FromRoute] Guid id)
    {
        UsuarioDto? usuarioDto = await _usuarioService.BuscarUsuario(id);
        if (usuarioDto == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        return Ok(usuarioDto);
    }
}
