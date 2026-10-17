namespace SistVenta.Api.DTOs;

public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}
