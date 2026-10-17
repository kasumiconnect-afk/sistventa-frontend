using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.Models;

public class Modulo
{
    [Key]
    public int IdModulo { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Icono { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Url { get; set; } = string.Empty;

    public int Orden { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}
