namespace SistVenta.Api.DTOs;

public class CreateCotizacionDto
{
    public int IdUsuario { get; set; }
    public int? IdCliente { get; set; }
    public int ValidezDias { get; set; } = 7;
    public string Notas { get; set; } = string.Empty;
    public string TipoEntrega { get; set; } = "TIENDA";
    public string? DireccionEntrega { get; set; }
    public decimal? CostoEnvio { get; set; } = 0;
    public DateTime? FechaEntregaProgramada { get; set; }
    public int? IdEntrega { get; set; }
    public List<CreateDetalleCotizacionDto> Detalles { get; set; } = new List<CreateDetalleCotizacionDto>();
}

public class CreateDetalleCotizacionDto
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; } = 0;
}
