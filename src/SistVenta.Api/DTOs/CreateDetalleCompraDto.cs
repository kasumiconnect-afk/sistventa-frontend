namespace SistVenta.Api.DTOs;

public class CreateDetalleCompraDto
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }
}
