using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class DetalleCotizacion
{
    [Key]
    public int IdDetalleCotizacion { get; set; }

    public int IdCotizacion { get; set; }

    [ForeignKey(nameof(IdCotizacion))]
    public Cotizacion? Cotizacion { get; set; }

    public int IdProducto { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Descuento { get; set; } = 0;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    // Campos de exhibición - NotMapped
    [NotMapped]
    public string CodigoSKU { get; set; } = string.Empty;

    [NotMapped]
    public string ProductoDescripcion { get; set; } = string.Empty;

    [NotMapped]
    public string Marca { get; set; } = string.Empty;

    [NotMapped]
    public string UnidadMedida { get; set; } = string.Empty;
}
