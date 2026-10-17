using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [MaxLength(20)]
    public string CedulaIdentidad { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [NotMapped]
    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
