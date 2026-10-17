using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class UpdateUsuarioDto
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

    public string? Contrasena { get; set; }

    public int IdRol { get; set; }

    public int Estado_id { get; set; }
}
