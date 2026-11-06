using Microsoft.AspNetCore.Mvc;
using nitynews.Dtos;
using nitynews.Exceptions;
using nitynews.Services.Interfaces;

namespace nitynews.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    
    [HttpPost("cadastrar")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] UsuarioDto dto, CancellationToken cancellationToken)
    {
        if (dto is null)
        {
            _logger.LogWarning("Tentativa de chamada do endpoint 'cadastrar' com payload nulo.");
            return BadRequest(ApiResponse.Falha("O corpo da requisição não pode ser vazio."));
        }

        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Campo inválido." : e.ErrorMessage)
                .ToList();

            _logger.LogWarning("Validação de modelo falhou ao tentar cadastrar usuário: {@Erros}", erros);
            return BadRequest(ApiResponse.Falha("Dados de requisição inválidos.", erros));
        }

        try
        {
            Guid idUsuario = await _usuarioService.CadastrarAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(BuscarUsuario), new { id = idUsuario }, ApiResponse<Guid>.Ok(idUsuario, "Usuário cadastrado com sucesso."));
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflito ao cadastrar usuário: {Message}", ex.Message);
            return Conflict(ApiResponse.Falha(ex.Message));
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Erro de negócio ao cadastrar usuário: {Message}", ex.Message);
            return BadRequest(ApiResponse.Falha(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao cadastrar usuário com email: {Email}", dto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse.Falha("Ocorreu um erro interno ao processar o cadastro do usuário."));
        }
    }

    
    [HttpGet("buscar/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarUsuario([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Chamada ao endpoint 'buscar/id' com GUID vazio.");
            return BadRequest(ApiResponse.Falha("O identificador informado não pode ser vazio."));
        }

        try
        {
            UsuarioDto? usuarioDto = await _usuarioService.BuscarUsuario(id, cancellationToken);
            if (usuarioDto == null)
            {
                return NotFound(ApiResponse<UsuarioDto>.Falha("Usuário não encontrado."));
            }

            return Ok(ApiResponse<UsuarioDto>.Ok(usuarioDto, "Usuário recuperado com sucesso."));
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Erro de negócio ao buscar usuário {Id}: {Message}", id, ex.Message);
            return BadRequest(ApiResponse.Falha(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar usuário pelo ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse.Falha("Ocorreu um erro interno ao buscar o usuário."));
        }
    }

   
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(ApiResponse<List<UsuarioDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarTodosUsuarios(CancellationToken cancellationToken)
    {
        try
        {
            List<UsuarioDto> usuarios = await _usuarioService.BuscarTodosUsuarios(cancellationToken);
            return Ok(ApiResponse<List<UsuarioDto>>.Ok(usuarios, "Usuários listados com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar todos os usuários.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse.Falha("Ocorreu um erro interno ao listar os usuários."));
        }
    }

    
    [HttpPut("delete/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletarUsuario([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Chamada ao endpoint 'delete/id' com GUID vazio.");
            return BadRequest(ApiResponse.Falha("O identificador informado não pode ser vazio."));
        }

        try
        {
            UsuarioDto? usuarioDto = await _usuarioService.DeletarUsuario(id, cancellationToken);
            if (usuarioDto == null)
            {
                return NotFound(ApiResponse<UsuarioDto>.Falha("Usuário não encontrado para desativação."));
            }

            return Ok(ApiResponse<UsuarioDto>.Ok(usuarioDto, "Usuário desativado com sucesso."));
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Erro de negócio ao desativar usuário {Id}: {Message}", id, ex.Message);
            return BadRequest(ApiResponse.Falha(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao desativar usuário com ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse.Falha("Ocorreu um erro interno ao desativar o usuário."));
        }
    }
}
