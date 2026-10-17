using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class MetodoPagoVenta
{
    [Key]
    public int IdMetodoPago { get; set; }
    
    public int IdVenta { get; set; }
    
    [ForeignKey(nameof(IdVenta))]
    public Venta? Venta { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TipoPago { get; set; } = string.Empty; // EFECTIVO, QR, TARJETA, TRANSFERENCIA, etc.
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }
    
    [MaxLength(100)]
    public string? ReferenciaTransaccion { get; set; } // Para QR, tarjeta, etc.
    
    [MaxLength(20)]
    public string Estado { get; set; } = "COMPLETADO"; // COMPLETADO, PENDIENTE, FALLIDO
    
    public DateTime FechaPago { get; set; } = DateTime.Now;
    
    [MaxLength(50)]
    public string? Banco { get; set; } // Para tarjetas/transferencias
    
    [MaxLength(20)]
    public string? Ultimos4Digitos { get; set; } // Para tarjetas
    
    public int Estado_id { get; set; } = 1; // Para soft delete u otros estados

    public DateTime Fecha { get; set; } = DateTime.Now;
}