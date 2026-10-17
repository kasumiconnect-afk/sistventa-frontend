using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateProveedorDto
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Contacto { get; set; }

    [MaxLength(20)]
    public string? Celular { get; set; }

    [MaxLength(100)]
    public string? Correo { get; set; }

    [MaxLength(200)]
    public string? Direccion { get; set; }

    public int Estado_id { get; set; }
}
