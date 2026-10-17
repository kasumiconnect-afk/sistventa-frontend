namespace SistVenta.Api.DTOs;

public class DetalleCotizacionDto
{
    public int IdDetalleCotizacion { get; set; }
    public int IdCotizacion { get; set; }
    public int IdProducto { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string ProductoDescripcion { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
}

// DTO para respuesta completa con subtotal calculado (usando VIEW vw_detalle_cotizacion_con_subtotal)
public class DetalleCotizacionCompletaDto : DetalleCotizacionDto
{
    public decimal Subtotal { get; set; }
}
