namespace SistVenta.Api.DTOs;

public class VentaDto
{
    public int IdVenta { get; set; }
    public int IdUsuario { get; set; }
    public int? IdCliente { get; set; }
    public int TipoComprobante { get; set; }
    public decimal ImporteRecibido { get; set; }
    public string? TipoPago { get; set; }
    public bool EsPagoMultiple { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public int? IdEntrega { get; set; }
    public List<DetalleVentaDto> Detalles { get; set; } = new();
    public List<MetodoPagoVentaDto> MetodosPago { get; set; } = new();
}

// DTO para respuesta completa con datos calculados (usando VIEW vw_venta_completa)
public class VentaCompletaDto : VentaDto
{
    public string? ClienteNombre { get; set; }
    public int? Cantidad { get; set; }
    public decimal? Total { get; set; }
    public decimal? ImporteCambio { get; set; }
    public string? TipoEntrega { get; set; }
    public string? DireccionEntrega { get; set; }
    public decimal? CostoEnvio { get; set; }
    public DateTime? FechaEntregaProgramada { get; set; }
    public string? EstadoEntrega { get; set; }
    public new List<DetalleVentaCompletaDto> Detalles { get; set; } = new();
}
