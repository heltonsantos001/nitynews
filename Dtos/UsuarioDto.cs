using System.ComponentModel.DataAnnotations;

namespace nitynews.Dtos;

public class UsuarioDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    public bool? Ativo { get; set; }

    public DateTime? DataCadastro { get; set; }

    public bool? EmailVerificado { get; set; }
}

