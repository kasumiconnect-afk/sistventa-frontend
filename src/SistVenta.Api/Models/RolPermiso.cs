using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

[Table("rolpermiso")]
public class RolPermiso
{
    [Key]
    public int IdRolPermiso { get; set; }

    public int IdRol { get; set; }

    [ForeignKey(nameof(IdRol))]
    public Rol? Rol { get; set; }

    public int IdPermiso { get; set; }

    [ForeignKey(nameof(IdPermiso))]
    public Permiso? Permiso { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
