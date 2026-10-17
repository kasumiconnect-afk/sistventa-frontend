namespace SistVenta.Api.DTOs;

public class ProveedorDto
{
    public int IdProveedor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Celular { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}
