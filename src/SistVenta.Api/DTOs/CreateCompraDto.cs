namespace SistVenta.Api.DTOs;

public class CreateCompraDto
{
    public int? IdUsuario { get; set; }
    public int IdProveedor { get; set; }
    public List<CreateDetalleCompraDto> Detalles { get; set; } = new();
}
