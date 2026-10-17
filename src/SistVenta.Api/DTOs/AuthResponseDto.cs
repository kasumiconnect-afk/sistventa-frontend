namespace SistVenta.Api.DTOs;

public class AuthResponseDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string Correo { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public string? RolNombre { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expiracion { get; set; }
    public List<string> Permisos { get; set; } = new List<string>();
}

public class RefreshTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
