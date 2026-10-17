namespace SistVenta.Api.DTOs;

public class TiendaDto
{
    public int IdTienda { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}
