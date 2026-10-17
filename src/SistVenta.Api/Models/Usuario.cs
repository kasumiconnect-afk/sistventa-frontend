using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Contrasena { get; set; } = string.Empty;

    public int IdRol { get; set; }

    [ForeignKey(nameof(IdRol))]
    public Rol? Rol { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [NotMapped]
    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
