namespace SistVenta.Api.DTOs;

public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string Correo { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public string? RolNombre { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
}
