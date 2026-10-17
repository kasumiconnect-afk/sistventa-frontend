using System.ComponentModel.DataAnnotations;

namespace SistVenta.Api.Models;

public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public int Estado_id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
