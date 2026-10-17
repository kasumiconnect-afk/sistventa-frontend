namespace SistVenta.Api.DTOs;

public class ProductoDto
{
    public int IdProducto { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? Presentacion { get; set; }
    public string? UnidadMedida { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public string? UbicacionAlmacen { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal Costopromedio { get; set; }
    public decimal MargenPorcentaje { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioMayorista { get; set; }
    public int IdCategoria { get; set; }
    public string? CategoriaNombre { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public bool StockBajo { get; set; }
}
