namespace SistVenta.Api.DTOs;

public class CotizacionDto
{
    public int IdCotizacion { get; set; }
    public int IdUsuario { get; set; }
    public int? IdCliente { get; set; }
    public int? Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime Validez { get; set; }
    public string Notas { get; set; } = string.Empty;
    public int? IdEntrega { get; set; }
    public List<DetalleCotizacionDto> Detalles { get; set; } = new List<DetalleCotizacionDto>();
}

// DTO para respuesta completa con datos calculados (usando VIEW vw_cotizacion_completa)
public class CotizacionCompletaDto : CotizacionDto
{
    public string? ClienteNombre { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? DescuentoTotal { get; set; }
    public decimal? Total { get; set; }
    public string? TipoEntrega { get; set; }
    public string? DireccionEntrega { get; set; }
    public decimal? CostoEnvio { get; set; }
    public DateTime? FechaEntregaProgramada { get; set; }
    public string? EstadoEntrega { get; set; }
    public new List<DetalleCotizacionCompletaDto> Detalles { get; set; } = new();
}
