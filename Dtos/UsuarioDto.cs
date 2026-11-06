using System.ComponentModel.DataAnnotations;

namespace nitynews.Dtos;

/// <summary>
/// DTO para transferência de dados de usuário.
/// </summary>
public class UsuarioDto
{
    /// <summary>
    /// Identificador único do usuário (gerado pelo sistema).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo 'Nome' deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do usuário.
    /// </summary>
    [Required(ErrorMessage = "O campo 'Email' é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo 'Email' não contém um formato de e-mail válido.")]
    [MaxLength(254, ErrorMessage = "O campo 'Email' não pode ter mais de 254 caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Situação cadastral do usuário (ativo/inativo).
    /// </summary>
    public bool? Ativo { get; set; }

    /// <summary>
    /// Data e hora do cadastro em UTC.
    /// </summary>
    public DateTime? DataCadastro { get; set; }

    /// <summary>
    /// Indica se o e-mail foi verificado via confirmação.
    /// </summary>
    public bool? EmailVerificado { get; set; }

    /// <summary>
    /// Data de desativação/cancelamento da conta.
    /// </summary>
    public DateTime? DataCancelamento { get; set; }
}
