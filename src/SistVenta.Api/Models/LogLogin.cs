using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistVenta.Api.Models;

public class LogLogin
{
    [Key]
    public int IdLogLogin { get; set; }

    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    public bool Exitoso { get; set; }

    [MaxLength(50)]
    public string IP { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Now;
}
