using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class Venta
{
    [Key]
    public int IdVenta { get; set; }

    public int IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; set; }

    public int? IdCliente { get; set; }

    [ForeignKey(nameof(IdCliente))]
    public Cliente? Cliente { get; set; }

    public int TipoComprobante { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ImporteRecibido { get; set; }

    [MaxLength(50)]
    public string TipoPago { get; set; } = string.Empty;

    public bool EsPagoMultiple { get; set; } = false;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int? IdEntrega { get; set; }

    [ForeignKey(nameof(IdEntrega))]
    public Entrega? Entrega { get; set; }

    // Navigation property
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    // Navigation property para métodos de pago
    public ICollection<MetodoPagoVenta> MetodosPago { get; set; } = new List<MetodoPagoVenta>();
}
