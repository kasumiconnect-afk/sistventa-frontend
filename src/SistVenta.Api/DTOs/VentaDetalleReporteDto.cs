using System;
using System.Collections.Generic;

namespace SistVenta.Api.DTOs
{
    public class VentaDetalleReporteDto
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public string NombreCliente { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal TotalVenta { get; set; }
        public List<DetalleVentaReporteItemDto> Items { get; set; }
    }
}