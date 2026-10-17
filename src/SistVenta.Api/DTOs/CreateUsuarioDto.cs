using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateUsuarioDto
{
    [Required]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Apellidos { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Contrasena { get; set; } = string.Empty;

    public int IdRol { get; set; }

    public int Estado_id { get; set; }
}
