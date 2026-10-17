namespace SistVenta.Api.DTOs;

public class CreateVentaDto
{
    public int IdUsuario { get; set; }
    public int? IdCliente { get; set; }
    public int TipoComprobante { get; set; }
    public decimal ImporteRecibido { get; set; }
    public string? TipoPago { get; set; } // EFECTIVO, QR, MULTIPLE, etc.
    public bool EsPagoMultiple { get; set; } = false;
    public List<CreateMetodoPagoDto> MetodosPago { get; set; } = new();
    public int? IdEntrega { get; set; }
    public List<CreateDetalleVentaDto> Detalles { get; set; } = new();
}

public class CreateMetodoPagoDto
{
    public string TipoPago { get; set; } = string.Empty; // EFECTIVO, QR, TARJETA, etc.
    public decimal Monto { get; set; }
    public string? ReferenciaTransaccion { get; set; }
    public string? Banco { get; set; }
    public string? Ultimos4Digitos { get; set; }
}
