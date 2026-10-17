namespace SistVenta.Api.DTOs;

public class DetalleCompraDto
{
    public int IdDetalleCompra { get; set; }
    public int IdProducto { get; set; }
    public string? CodigoSKU { get; set; }
    public string? ProductoDescripcion { get; set; }
    public string? Marca { get; set; }
    public string? UnidadMedida { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }
}

// DTO para respuesta completa con total calculado (usando VIEW vw_detallecompra_con_total)
public class DetalleCompraCompletaDto : DetalleCompraDto
{
    public decimal Total { get; set; }
}
