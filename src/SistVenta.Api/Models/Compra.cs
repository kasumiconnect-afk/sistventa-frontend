using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Compra
{
    [Key]
    public int IdCompra { get; set; }

    public int IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; set; }

    public int IdProveedor { get; set; }

    [ForeignKey(nameof(IdProveedor))]
    public Proveedor? Proveedor { get; set; }

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
}
