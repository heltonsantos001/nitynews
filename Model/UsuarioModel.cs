using System.ComponentModel.DataAnnotations;
using System;

namespace nitynews.Model;

public class Usuario
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
	
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public bool EmailVerificado { get; set; } = false;

    [Required]
    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public DateTime? DataUltimoEnvio { get; set; }

    public DateTime? DataCancelamento { get; set; }
}
