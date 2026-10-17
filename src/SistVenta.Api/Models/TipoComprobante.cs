using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.Models;

public class TipoComprobante
{
    [Key]
    public int IdTipoComprobante { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public int Estado_id { get; set; }
}
