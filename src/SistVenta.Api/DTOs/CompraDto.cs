namespace SistVenta.Api.DTOs;

public class CompraDto
{
    public int IdCompra { get; set; }
    public int? IdUsuario { get; set; }
    public int IdProveedor { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
    public List<DetalleCompraDto> Detalles { get; set; } = new();
}

// DTO para respuesta completa con datos calculados (usando VIEW vw_compra_completa)
public class CompraCompletaDto : CompraDto
{
    public string? ProveedorNombre { get; set; }
    public decimal? Total { get; set; }
    public new List<DetalleCompraCompletaDto> Detalles { get; set; } = new();
}
