using System.Net.Mail;
using nitynews.Dtos;
using nitynews.Exceptions;
using nitynews.Model;
using nitynews.Repository.Interfaces;
using nitynews.Services.Interfaces;

namespace nitynews.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> CadastrarAsync(UsuarioDto dto, CancellationToken cancellationToken = default)
    {
        if (dto is null)
        {
            _logger.LogWarning("Tentativa de cadastro com dados nulos.");
            throw new BusinessException("Os dados do usuário devem ser informados.");
        }

        // Validação e normalização dos dados recebidos
        ValidarDadosCadastro(dto);

        string emailNormalizado = dto.Email.Trim().ToLowerInvariant();
        string nomeNormalizado = dto.Nome.Trim();

        try
        {
            // Verificação de duplicidade de e-mail
            bool emailExiste = await _usuarioRepository.ExisteEmailAsync(emailNormalizado, null, cancellationToken);
            if (emailExiste)
            {
                _logger.LogWarning("Tentativa de cadastro com e-mail duplicado: {Email}", emailNormalizado);
                throw new ConflictException($"Já existe um usuário cadastrado com o e-mail '{emailNormalizado}'.");
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = nomeNormalizado,
                Email = emailNormalizado,
                Ativo = dto.Ativo ?? true,
                EmailVerificado = dto.EmailVerificado ?? false,
                DataCadastro = DateTime.UtcNow
            };

            Guid idCriado = await _usuarioRepository.CadastrarUsuarioAsync(usuario, cancellationToken);
            _logger.LogInformation("Usuário {Id} cadastrado com sucesso com e-mail {Email}.", idCriado, emailNormalizado);
            return idCriado;
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o cadastro do usuário para o e-mail {Email}", emailNormalizado);
            throw;
        }
    }

    public async Task<UsuarioDto?> BuscarUsuario(Guid idUsuario, CancellationToken cancellationToken = default)
    {
        if (idUsuario == Guid.Empty)
        {
            _logger.LogWarning("Busca de usuário acionada com ID Guid.Empty.");
            throw new BusinessException("O identificador do usuário informado é inválido.");
        }

        try
        {
            var usuario = await _usuarioRepository.BuscarUsuario(idUsuario, cancellationToken);
            if (usuario == null)
            {
                _logger.LogInformation("Usuário não encontrado para o ID {Id}.", idUsuario);
                return null;
            }

            return MapearParaDto(usuario);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário pelo ID {Id}", idUsuario);
            throw;
        }
    }

    public async Task<List<UsuarioDto>> BuscarTodosUsuarios(CancellationToken cancellationToken = default)
    {
        try
        {
            var usuarios = await _usuarioRepository.BuscarTodosUsuarios(cancellationToken);
            return usuarios.Select(MapearParaDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os usuários.");
            throw;
        }
    }

    public async Task<UsuarioDto?> DeletarUsuario(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Deleção de usuário acionada com ID Guid.Empty.");
            throw new BusinessException("O identificador do usuário informado é inválido.");
        }

        try
        {
            Usuario? usuario = await _usuarioRepository.BuscarUsuario(id, cancellationToken);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário com ID {Id} não encontrado para desativação.", id);
                return null;
            }

            if (!usuario.Ativo)
            {
                _logger.LogInformation("Usuário {Id} já estava desativado.", id);
                return MapearParaDto(usuario);
            }

            usuario.Ativo = false;
            usuario.DataCancelamento = DateTime.UtcNow;

            await _usuarioRepository.AtualizarUsuarioAsync(usuario, cancellationToken);
            _logger.LogInformation("Usuário com ID {Id} foi desativado com sucesso.", id);

            return MapearParaDto(usuario);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar usuário com ID {Id}", id);
            throw;
        }
    }

    private static void ValidarDadosCadastro(UsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new BusinessException("O nome do usuário é obrigatório e não pode conter apenas espaços em branco.");
        }

        string nome = dto.Nome.Trim();
        if (nome.Length < 2 || nome.Length > 100)
        {
            throw new BusinessException("O nome do usuário deve conter entre 2 e 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new BusinessException("O e-mail do usuário é obrigatório.");
        }

        string email = dto.Email.Trim();
        if (email.Length > 254)
        {
            throw new BusinessException("O e-mail do usuário não pode exceder 254 caracteres.");
        }

        if (!ValidarFormatoEmail(email))
        {
            throw new BusinessException("O formato do e-mail informado é inválido.");
        }
    }

    private static bool ValidarFormatoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            var mailAddress = new MailAddress(email);
            // Verifica se possui parte de host válida com ponto
            return mailAddress.Address == email && mailAddress.Host.Contains('.');
        }
        catch
        {
            return false;
        }
    }

    private static UsuarioDto MapearParaDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro,
            EmailVerificado = usuario.EmailVerificado,
            DataCancelamento = usuario.DataCancelamento
        };
    }
}
