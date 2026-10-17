namespace SistVenta.Api.DTOs;

public class RolDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}
