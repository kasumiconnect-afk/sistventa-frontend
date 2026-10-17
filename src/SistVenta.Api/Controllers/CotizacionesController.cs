using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CotizacionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CotizacionesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Cotizaciones?fechaInicio=...&fechaFin=...&estado=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CotizacionCompletaDto>>> GetCotizaciones(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] string? estado = null)
    {
        var inicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
        var fin = fechaFin ?? DateTime.Today;

        var query = _context.Cotizaciones
            .Include(c => c.Cliente)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
            .Where(c => c.Fecha.Date >= inicio.Date && c.Fecha.Date <= fin.Date);

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(c => c.Estado_id == int.Parse(estado));
        }

        var cotizaciones = await query
            .OrderByDescending(c => c.IdCotizacion)
            .Select(c => new CotizacionCompletaDto
            {
                IdCotizacion = c.IdCotizacion,
                IdUsuario = c.IdUsuario,
                IdCliente = c.IdCliente,
                ClienteNombre = c.Cliente != null ? c.Cliente.NombreCompleto : null,
                Subtotal = c.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
                DescuentoTotal = c.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario * d.Descuento / 100),
                Total = c.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario * (1 - d.Descuento / 100)),
                Estado_id = c.Estado_id,
                Fecha = c.Fecha,
                Validez = c.Validez,
                Notas = c.Notas,
                Detalles = c.Detalles.Select(d => new DetalleCotizacionCompletaDto
                {
                    IdDetalleCotizacion = d.IdDetalleCotizacion,
                    IdCotizacion = d.IdCotizacion,
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto!.CodigoSKU,
                    ProductoDescripcion = d.Producto.Descripcion,
                    Marca = d.Producto.Marca,
                    UnidadMedida = d.Producto.UnidadMedida,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = d.Descuento,
                    Subtotal = d.Cantidad * d.PrecioUnitario * (1 - d.Descuento / 100)
                }).ToList()
            })
            .ToListAsync();

        return cotizaciones;
    }

    // GET: api/Cotizaciones/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CotizacionCompletaDto>> GetCotizacion(int id)
    {
        var cotizacion = await _context.Cotizaciones
            .Include(c => c.Cliente)
            .Include(c => c.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(c => c.IdCotizacion == id);

        if (cotizacion == null)
        {
            return NotFound();
        }

        var cotizacionDto = new CotizacionCompletaDto
        {
            IdCotizacion = cotizacion.IdCotizacion,
            IdUsuario = cotizacion.IdUsuario,
            IdCliente = cotizacion.IdCliente,
            ClienteNombre = cotizacion.Cliente != null ? cotizacion.Cliente.NombreCompleto : null,
            Subtotal = cotizacion.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
            DescuentoTotal = cotizacion.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario * d.Descuento / 100),
            Total = cotizacion.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario * (1 - d.Descuento / 100)),
            Estado_id = cotizacion.Estado_id,
            Fecha = cotizacion.Fecha,
            Validez = cotizacion.Validez,
            Notas = cotizacion.Notas,
            Detalles = cotizacion.Detalles.Select(d => new DetalleCotizacionCompletaDto
            {
                IdDetalleCotizacion = d.IdDetalleCotizacion,
                IdCotizacion = d.IdCotizacion,
                IdProducto = d.IdProducto,
                CodigoSKU = d.Producto!.CodigoSKU,
                ProductoDescripcion = d.Producto.Descripcion,
                Marca = d.Producto.Marca,
                UnidadMedida = d.Producto.UnidadMedida,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Descuento = d.Descuento,
                Subtotal = d.Cantidad * d.PrecioUnitario * (1 - d.Descuento / 100)
            }).ToList()
        };

        return cotizacionDto;
    }

    // POST: api/Cotizaciones
    [HttpPost]
    public async Task<ActionResult<CotizacionDto>> PostCotizacion(CreateCotizacionDto dto)
    {
        if (dto.Detalles == null || dto.Detalles.Count == 0)
        {
            return BadRequest("La cotización debe tener al menos un detalle");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            decimal subtotal = 0;
            decimal descuentoTotal = 0;

            // Crear la cotización
            var cotizacion = new Cotizacion
            {
                IdUsuario = dto.IdUsuario,
                IdCliente = dto.IdCliente,
                Estado_id = 1,
                Fecha = DateTime.Now,
                Validez = DateTime.Now.AddDays(dto.ValidezDias),
                Notas = dto.Notas,
                IdEntrega = dto.IdEntrega,
                Detalles = new List<DetalleCotizacion>()
            };

            // Procesar detalles
            foreach (var detDto in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detDto.IdProducto);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {detDto.IdProducto} no encontrado");
                }

                // Usar precio del producto si no viene en el DTO
                var precioUnitario = detDto.PrecioUnitario <= 0 ? producto.PrecioVenta : detDto.PrecioUnitario;
                var descuento = detDto.Descuento;
                var subtotalItem = Math.Round(precioUnitario * detDto.Cantidad, 2);
                var subtotalConDescuento = Math.Round(subtotalItem - (subtotalItem * descuento / 100), 2);

                // Crear detalle
                var detalle = new DetalleCotizacion
                {
                    IdProducto = detDto.IdProducto,
                    Cantidad = detDto.Cantidad,
                    PrecioUnitario = precioUnitario,
                    Descuento = descuento,
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };

                cotizacion.Detalles.Add(detalle);
                subtotal += subtotalItem;
                descuentoTotal += (subtotalItem * descuento / 100);
            }

            // Guardar cotización y detalles
            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();

            // Cargar los datos completos para retornar
            await _context.Entry(cotizacion).Reference(c => c.Cliente).LoadAsync();
            await _context.Entry(cotizacion).Collection(c => c.Detalles).LoadAsync();
            foreach (var det in cotizacion.Detalles)
            {
                await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
            }

            var cotizacionDto = new CotizacionCompletaDto
            {
                IdCotizacion = cotizacion.IdCotizacion,
                IdUsuario = cotizacion.IdUsuario,
                IdCliente = cotizacion.IdCliente,
                ClienteNombre = cotizacion.Cliente != null ? cotizacion.Cliente.NombreCompleto : null,
                Subtotal = Math.Round(subtotal, 2),
                DescuentoTotal = Math.Round(descuentoTotal, 2),
                Total = Math.Round(subtotal - descuentoTotal + (dto.CostoEnvio ?? 0m), 2),
                Estado_id = cotizacion.Estado_id,
                Fecha = cotizacion.Fecha,
                Validez = cotizacion.Validez,
                Notas = cotizacion.Notas,
                Detalles = cotizacion.Detalles.Select(d => new DetalleCotizacionCompletaDto
                {
                    IdDetalleCotizacion = d.IdDetalleCotizacion,
                    IdCotizacion = d.IdCotizacion,
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto!.CodigoSKU,
                    ProductoDescripcion = d.Producto.Descripcion,
                    Marca = d.Producto.Marca,
                    UnidadMedida = d.Producto.UnidadMedida,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = d.Descuento,
                    Subtotal = d.Cantidad * d.PrecioUnitario * (1 - d.Descuento / 100)
                }).ToList()
            };

            return CreatedAtAction(nameof(GetCotizacion), new { id = cotizacion.IdCotizacion }, cotizacionDto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // PUT: api/Cotizaciones/5/estado
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] int nuevoEstado)
    {
        var cotizacion = await _context.Cotizaciones.FindAsync(id);
        if (cotizacion == null)
        {
            return NotFound();
        }

        var estadosValidos = new[] { 1, 2, 3, 4, 5, 6 };
        if (!estadosValidos.Contains(nuevoEstado))
        {
            return BadRequest("Estado inválido");
        }

        cotizacion.Estado_id = nuevoEstado;
        _context.Entry(cotizacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Cotizaciones/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCotizacion(int id)
    {
        var cotizacion = await _context.Cotizaciones
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c => c.IdCotizacion == id);

        if (cotizacion == null)
        {
            return NotFound();
        }

        if (cotizacion.Estado_id == 5) // 5 = CONVERTIDA
        {
            return BadRequest("No se puede eliminar una cotización convertida a venta");
        }

        _context.DetallesCotizacion.RemoveRange(cotizacion.Detalles);
        _context.Cotizaciones.Remove(cotizacion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Cotizaciones/5/convertir-a-venta
    [HttpPost("{id}/convertir-a-venta")]
    public async Task<ActionResult<VentaCompletaDto>> ConvertirAVenta(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (cotizacion.Estado_id == 5) // 5 = CONVERTIDA
            {
                return BadRequest("Esta cotización ya fue convertida a venta");
            }

            int cantidadTotal = 0;
            decimal total = 0;

            // Crear la venta
            var venta = new Venta
            {
                IdUsuario = cotizacion.IdUsuario,
                IdCliente = cotizacion.IdCliente,
                TipoComprobante = 1, // Factura por defecto
                TipoPago = "EFECTIVO",
                Estado_id = 1,
                Fecha = DateTime.Now,
                IdEntrega = cotizacion.IdEntrega,
                Detalles = new List<DetalleVenta>()
            };

            // Procesar detalles de la cotización
            foreach (var detCotizacion in cotizacion.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detCotizacion.IdProducto);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {detCotizacion.IdProducto} no encontrado");
                }

                // Validar stock
                if (producto.Stock < detCotizacion.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para el producto {producto.Descripcion}");
                }

                // Usar precio con descuento de la cotización
                var precioVenta = Math.Round(detCotizacion.PrecioUnitario * (1 - detCotizacion.Descuento / 100), 2);
                var subtotal = Math.Round(precioVenta * detCotizacion.Cantidad, 2);

                // Crear detalle de venta
                var detalleVenta = new DetalleVenta
                {
                    IdProducto = detCotizacion.IdProducto,
                    Cantidad = detCotizacion.Cantidad,
                    PrecioVenta = precioVenta,
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };

                venta.Detalles.Add(detalleVenta);
                cantidadTotal += detCotizacion.Cantidad;
                total += subtotal;

                // Reducir stock
                producto.Stock -= detCotizacion.Cantidad;
                _context.Entry(producto).State = EntityState.Modified;
            }

            // Guardar venta y detalles
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // Actualizar estado de la cotización
            cotizacion.Estado_id = 5; // CONVERTIDA
            _context.Entry(cotizacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();

            // Cargar los datos completos para retornar
            await _context.Entry(venta).Reference(v => v.Cliente).LoadAsync();
            await _context.Entry(venta).Collection(v => v.Detalles).LoadAsync();
            foreach (var det in venta.Detalles)
            {
                await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
            }

            var ventaDto = new VentaCompletaDto
            {
                IdVenta = venta.IdVenta,
                IdUsuario = venta.IdUsuario,
                IdCliente = venta.IdCliente,
                ClienteNombre = venta.Cliente != null ? venta.Cliente.NombreCompleto : null,
                TipoComprobante = venta.TipoComprobante,
                Cantidad = cantidadTotal,
                Total = Math.Round(total, 2),
                ImporteRecibido = Math.Round(total, 2),
                ImporteCambio = 0,
                TipoPago = venta.TipoPago,
                Estado_id = venta.Estado_id,
                Fecha = venta.Fecha,
                Detalles = venta.Detalles.Select(d => new DetalleVentaCompletaDto
                {
                    IdDetalleVenta = d.IdDetalleVenta,
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto!.CodigoSKU,
                    ProductoDescripcion = d.Producto.Descripcion,
                    Marca = d.Producto.Marca,
                    UnidadMedida = d.Producto.UnidadMedida,
                    Cantidad = d.Cantidad,
                    PrecioVenta = d.PrecioVenta,
                    Subtotal = d.Cantidad * d.PrecioVenta
                }).ToList()
            };

            return CreatedAtAction("GetVenta", "Ventas", new { id = venta.IdVenta }, ventaDto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
