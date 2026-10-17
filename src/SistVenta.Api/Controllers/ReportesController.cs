using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("ventas-diarias")]
    public async Task<ActionResult<ReporteVentasDiarias>> GetVentasDiarias([FromQuery] DateTime? fecha = null)
    {
        var fechaConsulta = fecha ?? DateTime.Today;

        Console.WriteLine($"Fecha recibida: {fecha}, Fecha consulta: {fechaConsulta}");

        var reporte = new ReporteVentasDiarias
        {
            Fecha = fechaConsulta,
            TotalVentas = await ObtenerTotalVentas(fechaConsulta),
            CantidadTransacciones = await ObtenerCantidadTransacciones(fechaConsulta),
            TotalVentasEfectivo = await ObtenerTotalVentasEfectivo(fechaConsulta),
            TotalVentasQR = await ObtenerTotalVentasQR(fechaConsulta)
        };

        Console.WriteLine($"Reporte generado: {reporte.TotalVentas}, {reporte.CantidadTransacciones}");

        return reporte;
    }

    [HttpGet("ventas-ultimos-dias")]
    public async Task<ActionResult<IEnumerable<ReporteVentasDiarias>>> GetVentasUltimosDias([FromQuery] int dias = 7)
    {
        var reportes = new List<ReporteVentasDiarias>();
        var fechaFin = DateTime.Today;

        for (int i = 0; i < dias; i++)
        {
            var fecha = fechaFin.AddDays(-i);

            var reporte = new ReporteVentasDiarias
            {
                Fecha = fecha,
                TotalVentas = await ObtenerTotalVentas(fecha),
                CantidadTransacciones = await ObtenerCantidadTransacciones(fecha),
                TotalVentasEfectivo = await ObtenerTotalVentasEfectivo(fecha),
                TotalVentasQR = await ObtenerTotalVentasQR(fecha)
            };

            reportes.Add(reporte);
        }

        return reportes.OrderByDescending(r => r.Fecha).ToList();
    }

    [HttpGet("stock-bajo")]
    public async Task<ActionResult<IEnumerable<Producto>>> GetStockBajo()
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Stock <= p.StockMinimo && p.Estado_id == 1)
            .OrderBy(p => p.Stock)
            .ToListAsync();

        return productos;
    }

    private async Task<decimal> ObtenerTotalVentas(DateTime fecha)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Detalles)
            .Where(v => v.Fecha.Date == fecha.Date && v.Estado_id == 2)
            .ToListAsync();

        return ventas.Sum(v => v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta));
    }

    private async Task<int> ObtenerCantidadTransacciones(DateTime fecha)
    {
        return await _context.Ventas
            .Where(v => v.Fecha.Date == fecha.Date && v.Estado_id == 2)
            .CountAsync();
    }

    private async Task<decimal> ObtenerTotalVentasEfectivo(DateTime fecha)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Detalles)
            .Where(v => v.Fecha.Date == fecha.Date && v.Estado_id == 2 && v.TipoPago.ToUpper() == "EFECTIVO")
            .ToListAsync();

        return ventas.Sum(v => v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta));
    }

    private async Task<decimal> ObtenerTotalVentasQR(DateTime fecha)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Detalles)
            .Where(v => v.Fecha.Date == fecha.Date && v.Estado_id == 2 && v.TipoPago.ToUpper() == "QR")
            .ToListAsync();

        return ventas.Sum(v => v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta));
    }



    [HttpGet("ventas-detalladas-dia")]
    public async Task<ActionResult<PaginatedResultDto<VentaDetalleReporteDto>>> GetVentasDetalladasDia([FromQuery] DateTime? fecha = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 7)
    {
        var fechaConsulta = fecha ?? DateTime.Today;

        var query = _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .Where(v => v.Fecha.Date == fechaConsulta.Date && v.Estado_id == 2);

        var totalCount = await query.CountAsync();

        var ventas = await query
            .OrderBy(v => v.Fecha)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VentaDetalleReporteDto
            {
                IdVenta = v.IdVenta,
                FechaVenta = v.Fecha,
                NombreCliente = v.Cliente != null ? v.Cliente.NombreCompleto : "Consumidor Final",
                TipoComprobante = v.TipoComprobante.ToString(),
                NumeroComprobante = "N/A", // Suponiendo que no hay un campo para esto en Venta
                TotalVenta = v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta),
                Items = v.Detalles.Select(d => new DetalleVentaReporteItemDto
                {
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto != null ? d.Producto.CodigoSKU : "",
                    DescripcionProducto = d.Producto != null ? d.Producto.Descripcion : "",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioVenta,
                    Subtotal = d.Cantidad * d.PrecioVenta
                }).ToList()
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResultDto<VentaDetalleReporteDto>
        {
            Items = ventas,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };

        return Ok(paginatedResult);
    }

    [HttpGet("productos-mas-vendidos")]
    public async Task<ActionResult<IEnumerable<ProductoReporteDTO>>> GetProductosMasVendidos(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var inicio = fechaInicio ?? DateTime.MinValue;
        var fin = fechaFin ?? DateTime.MaxValue;

        var productosMasVendidos = await _context.DetallesVenta
            .Include(dv => dv.Producto)
            .Include(dv => dv.Venta)
                .ThenInclude(v => v.Cliente)
            .Where(dv => dv.Venta.Fecha.Date >= inicio.Date && dv.Venta.Fecha.Date <= fin.Date && dv.Venta.Estado_id == 2)
            .GroupBy(dv => new { dv.Producto.IdProducto, dv.Producto.Descripcion })
            .Select(g => new ProductoReporteDTO
            {
                IdProducto = g.Key.IdProducto,
                NombreProducto = g.Key.Descripcion,
                CantidadVendida = g.Sum(dv => dv.Cantidad),
                TotalVendido = g.Sum(dv => dv.Cantidad * dv.PrecioVenta)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(10)
            .ToListAsync();

        return Ok(productosMasVendidos);
    }

    [HttpGet("productos-mas-comprados")]
    public async Task<ActionResult<IEnumerable<ProductoCompradoDTO>>> GetProductosMasComprados(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var inicio = fechaInicio ?? DateTime.MinValue;
        var fin = fechaFin ?? DateTime.MaxValue;

        var productosMasComprados = await _context.DetallesCompra
            .Include(dc => dc.Producto)
            .Include(dc => dc.Compra)
                .ThenInclude(c => c.Proveedor)
            .Where(dc => dc.Compra.Fecha.Date >= inicio.Date && dc.Compra.Fecha.Date <= fin.Date && dc.Compra.Estado_id == 1)
            .GroupBy(dc => new { dc.Producto.IdProducto, dc.Producto.Descripcion, dc.Compra.Proveedor.Nombre })
            .Select(g => new ProductoCompradoDTO
            {
                IdProducto = g.Key.IdProducto,
                NombreProducto = g.Key.Descripcion,
                CantidadComprada = g.Sum(dc => dc.Cantidad),
                TotalComprado = g.Sum(dc => dc.Cantidad * dc.PrecioCompra),
                ProveedorNombre = g.Key.Nombre
            })
            .OrderByDescending(p => p.CantidadComprada)
            .Take(10)
            .ToListAsync();

        return Ok(productosMasComprados);
    }
}
