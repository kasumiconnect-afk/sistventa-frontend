namespace SistVenta.Api.DTOs
{
    public class ProductoReporteDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }

    public class ProductoCompradoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadComprada { get; set; }
        public decimal TotalComprado { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
    }
}
