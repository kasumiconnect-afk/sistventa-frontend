namespace SistVenta.Api.DTOs;

public class ClienteDto
{
    public int IdCliente { get; set; }
    public string? CedulaIdentidad { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
}
