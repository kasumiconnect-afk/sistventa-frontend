using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Cotizacion
{
    [Key]
    public int IdCotizacion { get; set; }

    public int IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; set; }

    public int? IdCliente { get; set; }

    [ForeignKey(nameof(IdCliente))]
    public Cliente? Cliente { get; set; }

    public int? Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public DateTime Validez { get; set; } = DateTime.Now.AddDays(7);

    [MaxLength(500)]
    public string Notas { get; set; } = string.Empty;

    public int? IdEntrega { get; set; }

    [ForeignKey(nameof(IdEntrega))]
    public Entrega? Entrega { get; set; }

    // Navigation property
    public ICollection<DetalleCotizacion> Detalles { get; set; } = new List<DetalleCotizacion>();
}
