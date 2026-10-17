using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.Models;

public class Estado
{
    [Key]
    public int Estado_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;
}
