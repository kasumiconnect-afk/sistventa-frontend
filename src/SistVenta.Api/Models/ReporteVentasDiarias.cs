using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class ReporteVentasDiarias
{
    public DateTime Fecha { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalVentas { get; set; }

    public int CantidadTransacciones { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalVentasEfectivo { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalVentasQR { get; set; }

    [NotMapped]
    public decimal TicketPromedio => CantidadTransacciones > 0 ? TotalVentas / CantidadTransacciones : 0;
}
