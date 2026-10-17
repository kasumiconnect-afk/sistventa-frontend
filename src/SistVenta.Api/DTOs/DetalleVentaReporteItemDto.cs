namespace SistVenta.Api.DTOs
{
    public class DetalleVentaReporteItemDto
    {
        public int IdProducto { get; set; }
        public string CodigoSKU { get; set; }
        public string DescripcionProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}