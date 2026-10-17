using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class CreateProductoDto
{
    [Required]
    [MaxLength(50)]
    public string CodigoSKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Marca { get; set; }

    [MaxLength(100)]
    public string? Modelo { get; set; }

    [MaxLength(50)]
    public string? Presentacion { get; set; }

    [MaxLength(20)]
    public string? UnidadMedida { get; set; }

    public int Stock { get; set; }

    public int? StockMinimo { get; set; }

    [MaxLength(100)]
    public string? UbicacionAlmacen { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal Costopromedio { get; set; }

    public decimal MargenPorcentaje { get; set; }

    public decimal PrecioMayorista { get; set; }

    public int IdCategoria { get; set; }

    public int Estado_id { get; set; }
}
