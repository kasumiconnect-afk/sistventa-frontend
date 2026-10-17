using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.Models;

public class Proveedor
{
    [Key]
    public int IdProveedor { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Contacto { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Celular { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
