using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateTipoComprobanteDto
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public int Estado_id { get; set; }
}
