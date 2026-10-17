namespace SistVenta.Api.DTOs;

public class DetalleVentaDto
{
    public int IdDetalleVenta { get; set; }
    public int IdProducto { get; set; }
    public string? CodigoSKU { get; set; }
    public string? ProductoDescripcion { get; set; }
    public string? Marca { get; set; }
    public string? UnidadMedida { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioVenta { get; set; }
}

// DTO para respuesta completa con subtotal calculado (usando VIEW vw_detalle_venta_con_subtotal)
public class DetalleVentaCompletaDto : DetalleVentaDto
{
    public decimal Subtotal { get; set; }
}
