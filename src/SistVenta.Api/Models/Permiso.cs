using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

[Table("permiso")]
public class Permiso
{
    [Key]
    public int IdPermiso { get; set; }

    [Required]
    [MaxLength(100)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    public int IdModulo { get; set; }

    [ForeignKey(nameof(IdModulo))]
    public Modulo? Modulo { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
