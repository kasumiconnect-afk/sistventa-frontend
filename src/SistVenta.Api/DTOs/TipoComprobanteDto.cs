namespace SistVenta.Api.DTOs;

public class TipoComprobanteDto
{
    public int IdTipoComprobante { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Estado_id { get; set; }
}
