using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateTiendaDto
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Nit { get; set; }

    [MaxLength(200)]
    public string? Direccion { get; set; }

    [MaxLength(20)]
    public string? Telefono { get; set; }

    public int Estado_id { get; set; }
}
