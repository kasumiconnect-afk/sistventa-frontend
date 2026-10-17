using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateClienteDto
{
    [MaxLength(20)]
    public string? CedulaIdentidad { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Apellidos { get; set; }

    [MaxLength(200)]
    public string? Direccion { get; set; }

    [MaxLength(20)]
    public string? Telefono { get; set; }

    public int Estado_id { get; set; }
}
