using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Entrega
{
    [Key]
    public int IdEntrega { get; set; }

    [MaxLength(20)]
    public string TipoEntrega { get; set; } = "TIENDA";

    [MaxLength(500)]
    public string? DireccionEntrega { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CostoEnvio { get; set; } = 0;

    public DateTime? FechaEntregaProgramada { get; set; }

    [MaxLength(20)]
    public string? EstadoEntrega { get; set; }

    public int? Estado_id { get; set; }

    public DateTime? Fecha { get; set; }
}
