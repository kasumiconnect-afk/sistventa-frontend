namespace SistVenta.Api.DTOs;

public class MetodoPagoVentaDto
{
    public int IdMetodoPago { get; set; }
    public int IdVenta { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? ReferenciaTransaccion { get; set; }
    public string Estado { get; set; } = "COMPLETADO";
    public DateTime FechaPago { get; set; }
    public string? Banco { get; set; }
    public string? Ultimos4Digitos { get; set; }
}