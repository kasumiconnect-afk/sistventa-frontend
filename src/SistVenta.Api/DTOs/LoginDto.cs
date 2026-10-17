using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;
}
