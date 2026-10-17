using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

[Table("rol")]
public class Rol
{
    [Key]
    public int IdRol { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
