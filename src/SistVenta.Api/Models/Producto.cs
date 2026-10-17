using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Producto
{
    [Key]
    public int IdProducto { get; set; }

    [Required]
    [MaxLength(50)]
    public string CodigoSKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Marca { get; set; } = "Genérico";

    [MaxLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Presentacion { get; set; } = "Unidad";

    [MaxLength(20)]
    public string UnidadMedida { get; set; } = "Unidad";

    public int Stock { get; set; }

    public int StockMinimo { get; set; } = 5;

    [MaxLength(100)]
    public string UbicacionAlmacen { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioCompra { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Costopromedio { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal MargenPorcentaje { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioVenta { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioMayorista { get; set; }

    public int IdCategoria { get; set; }

    [ForeignKey(nameof(IdCategoria))]
    public Categoria? Categoria { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [NotMapped]
    public string NombreCompleto => $"[{CodigoSKU}] {Descripcion} - {Marca}";

    [NotMapped]
    public bool StockBajo => Stock <= StockMinimo;
}
