namespace SistVenta.Api.DTOs;

public class CreateDetalleVentaDto
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioVenta { get; set; }
}
