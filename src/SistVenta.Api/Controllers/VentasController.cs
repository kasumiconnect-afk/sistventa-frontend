using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VentasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Ventas?fechaInicio=...&fechaFin=...&idCliente=...&page=...&pageSize=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VentaCompletaDto>>> GetVentas(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] int? idCliente = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var inicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
        var fin = fechaFin ?? DateTime.Today;

        var query = _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Entrega)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(v => v.MetodosPago)
            .Where(v => v.Fecha.Date >= inicio.Date && v.Fecha.Date <= fin.Date);

        if (idCliente.HasValue)
        {
            query = query.Where(v => v.IdCliente == idCliente.Value);
        }

        var totalCount = await query.CountAsync();

        var ventas = await query
            .OrderByDescending(v => v.IdVenta)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VentaCompletaDto
            {
                IdVenta = v.IdVenta,
                IdUsuario = v.IdUsuario,
                IdCliente = v.IdCliente,
                ClienteNombre = v.Cliente != null ? v.Cliente.NombreCompleto : null,
                TipoComprobante = v.TipoComprobante,
                Cantidad = v.Detalles.Sum(d => d.Cantidad),
                Total = v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta),
                ImporteRecibido = v.ImporteRecibido,
                ImporteCambio = v.ImporteRecibido - v.Detalles.Sum(d => d.Cantidad * d.PrecioVenta),
                TipoPago = v.TipoPago,
                EsPagoMultiple = v.EsPagoMultiple,
                Estado_id = v.Estado_id,
                Fecha = v.Fecha,
                TipoEntrega = v.Entrega != null ? v.Entrega.TipoEntrega : null,
                DireccionEntrega = v.Entrega != null ? v.Entrega.DireccionEntrega : null,
                CostoEnvio = v.Entrega != null ? v.Entrega.CostoEnvio : null,
                FechaEntregaProgramada = v.Entrega != null ? v.Entrega.FechaEntregaProgramada : null,
                EstadoEntrega = v.Entrega != null ? v.Entrega.EstadoEntrega : null,
                IdEntrega = v.IdEntrega,
                Detalles = v.Detalles.Select(d => new DetalleVentaCompletaDto
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
                }).ToList(),
                MetodosPago = v.MetodosPago.Select(mp => new MetodoPagoVentaDto
                {
                    IdMetodoPago = mp.IdMetodoPago,
                    IdVenta = mp.IdVenta,
                    TipoPago = mp.TipoPago,
                    Monto = mp.Monto,
                    ReferenciaTransaccion = mp.ReferenciaTransaccion,
                    Estado = mp.Estado,
                    FechaPago = mp.FechaPago,
                    Banco = mp.Banco,
                    Ultimos4Digitos = mp.Ultimos4Digitos
                }).ToList()
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());

        return ventas;
    }

    // GET: api/Ventas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<VentaCompletaDto>> GetVenta(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Entrega)
            .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
            .Include(v => v.MetodosPago)
            .FirstOrDefaultAsync(v => v.IdVenta == id);

        if (venta == null)
        {
            return NotFound();
        }

        var cantidadTotal = venta.Detalles.Sum(d => d.Cantidad);
        var totalVenta = venta.Detalles.Sum(d => d.Cantidad * d.PrecioVenta);

        var ventaDto = new VentaCompletaDto
        {
            IdVenta = venta.IdVenta,
            IdUsuario = venta.IdUsuario,
            IdCliente = venta.IdCliente,
            ClienteNombre = venta.Cliente != null ? venta.Cliente.NombreCompleto : null,
            TipoComprobante = venta.TipoComprobante,
            Cantidad = cantidadTotal,
            Total = totalVenta,
            ImporteRecibido = venta.ImporteRecibido,
            ImporteCambio = venta.ImporteRecibido - totalVenta,
            TipoPago = venta.TipoPago,
            EsPagoMultiple = venta.EsPagoMultiple,
            Estado_id = venta.Estado_id,
            Fecha = venta.Fecha,
            TipoEntrega = venta.Entrega != null ? venta.Entrega.TipoEntrega : null,
            DireccionEntrega = venta.Entrega != null ? venta.Entrega.DireccionEntrega : null,
            CostoEnvio = venta.Entrega != null ? venta.Entrega.CostoEnvio : null,
            FechaEntregaProgramada = venta.Entrega != null ? venta.Entrega.FechaEntregaProgramada : null,
            EstadoEntrega = venta.Entrega != null ? venta.Entrega.EstadoEntrega : null,
            IdEntrega = venta.IdEntrega,
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
            }).ToList(),
            MetodosPago = venta.MetodosPago.Select(mp => new MetodoPagoVentaDto
            {
                IdMetodoPago = mp.IdMetodoPago,
                IdVenta = mp.IdVenta,
                TipoPago = mp.TipoPago,
                Monto = mp.Monto,
                ReferenciaTransaccion = mp.ReferenciaTransaccion,
                Estado = mp.Estado,
                FechaPago = mp.FechaPago,
                Banco = mp.Banco,
                Ultimos4Digitos = mp.Ultimos4Digitos
            }).ToList()
        };

        return ventaDto;
    }

    // PUT: api/Ventas/{id}/estado-entrega
    [HttpPut("{id}/estado-entrega")]
    public async Task<ActionResult<VentaCompletaDto>> ActualizarEstadoEntrega(int id, [FromBody] string nuevoEstado)
    {
        var venta = await _context.Ventas
            .Include(v => v.Entrega)
            .FirstOrDefaultAsync(v => v.IdVenta == id);

        if (venta == null)
        {
            return NotFound();
        }

        if (venta.Entrega == null)
        {
            return BadRequest("Esta venta no tiene información de entrega");
        }

        venta.Entrega.EstadoEntrega = nuevoEstado;
        _context.Entry(venta.Entrega).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Recargar con relaciones completas
        await _context.Entry(venta).Reference(v => v.Cliente).LoadAsync();
        await _context.Entry(venta).Collection(v => v.Detalles).LoadAsync();
        foreach (var det in venta.Detalles)
        {
            await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
        }

        var cantidadTotal = venta.Detalles.Sum(d => d.Cantidad);
        var totalVenta = venta.Detalles.Sum(d => d.Cantidad * d.PrecioVenta);

        var ventaDto = new VentaCompletaDto
        {
            IdVenta = venta.IdVenta,
            IdUsuario = venta.IdUsuario,
            IdCliente = venta.IdCliente,
            ClienteNombre = venta.Cliente != null ? venta.Cliente.NombreCompleto : null,
            TipoComprobante = venta.TipoComprobante,
            Cantidad = cantidadTotal,
            Total = totalVenta,
            ImporteRecibido = venta.ImporteRecibido,
            ImporteCambio = venta.ImporteRecibido - totalVenta,
            TipoPago = venta.TipoPago,
            Estado_id = venta.Estado_id,
            Fecha = venta.Fecha,
            TipoEntrega = venta.Entrega.TipoEntrega,
            DireccionEntrega = venta.Entrega.DireccionEntrega,
            CostoEnvio = venta.Entrega.CostoEnvio,
            FechaEntregaProgramada = venta.Entrega.FechaEntregaProgramada,
            EstadoEntrega = venta.Entrega.EstadoEntrega,
            IdEntrega = venta.IdEntrega,
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

        return ventaDto;
    }

    // POST: api/Ventas
    [HttpPost]
    public async Task<ActionResult<VentaDto>> PostVenta(CreateVentaDto dto)
    {
        // Validaciones básicas
        if (dto.Detalles == null || dto.Detalles.Count == 0)
        {
            return BadRequest("La venta debe tener al menos un detalle");
        }

        var tipoPago = dto.TipoPago?.Trim().ToUpper() ?? "EFECTIVO";
        
        // Validar tipo de pago
        if (dto.EsPagoMultiple)
        {
            tipoPago = "MULTIPLE";
            if (dto.MetodosPago == null || dto.MetodosPago.Count == 0)
            {
                return BadRequest("Para pago múltiple debe especificar al menos un método de pago");
            }
            
            // Validar que la suma de métodos de pago coincida con el total
            var sumaMetodos = dto.MetodosPago.Sum(mp => mp.Monto);
            if (Math.Abs(sumaMetodos - dto.ImporteRecibido) > 0.01m)
            {
                return BadRequest($"La suma de métodos de pago ({sumaMetodos}) no coincide con el importe recibido ({dto.ImporteRecibido})");
            }
        }
        else if (tipoPago != "EFECTIVO" && tipoPago != "QR")
        {
            return BadRequest("Tipo de pago inválido. Solo EFECTIVO, QR o MULTIPLE");
        }

        // Para pagos QR, inicialmente no descontamos stock hasta confirmación
        var esPagoPendiente = tipoPago == "QR";

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            int cantidadTotal = 0;
            decimal total = 0;

            // Crear la venta
            var venta = new Venta
            {
                IdUsuario = dto.IdUsuario,
                IdCliente = dto.IdCliente,
                TipoComprobante = dto.TipoComprobante,
                ImporteRecibido = dto.ImporteRecibido,
                TipoPago = tipoPago,
                EsPagoMultiple = dto.EsPagoMultiple,
                Estado_id = esPagoPendiente ? 1 : 2, // PENDIENTE si es QR, PAGADA si es efectivo
                Fecha = DateTime.Now,
                IdEntrega = dto.IdEntrega,
                Detalles = new List<DetalleVenta>(),
                MetodosPago = new List<MetodoPagoVenta>()
            };

            // Procesar detalles
            foreach (var detDto in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detDto.IdProducto);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {detDto.IdProducto} no encontrado");
                }

                // Validar stock
                if (producto.Stock < detDto.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para el producto {producto.Descripcion}");
                }

                // Usar precio del producto si no viene en el DTO
                var precioVenta = detDto.PrecioVenta <= 0 ? producto.PrecioVenta : detDto.PrecioVenta;
                var subtotal = Math.Round(precioVenta * detDto.Cantidad, 2);

                // Crear detalle
                var detalle = new DetalleVenta
                {
                    IdProducto = detDto.IdProducto,
                    Cantidad = detDto.Cantidad,
                    PrecioVenta = precioVenta,
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };

                venta.Detalles.Add(detalle);
                cantidadTotal += detDto.Cantidad;
                total += subtotal;

                // Reducir stock solo si no es pago pendiente (QR)
                if (!esPagoPendiente)
                {
                    producto.Stock -= detDto.Cantidad;
                    _context.Entry(producto).State = EntityState.Modified;
                }
            }

            // Agregar métodos de pago si es pago múltiple
            if (dto.EsPagoMultiple && dto.MetodosPago != null)
            {
                foreach (var metodoDto in dto.MetodosPago)
                {
                    var metodoPago = new MetodoPagoVenta
                    {
                        TipoPago = metodoDto.TipoPago.ToUpper(),
                        Monto = metodoDto.Monto,
                        ReferenciaTransaccion = metodoDto.ReferenciaTransaccion,
                        Estado = "COMPLETADO",
                        FechaPago = DateTime.Now,
                        Banco = metodoDto.Banco,
                        Ultimos4Digitos = metodoDto.Ultimos4Digitos,
                        Estado_id = 1,
                        Fecha = DateTime.Now
                    };
                    venta.MetodosPago.Add(metodoPago);
                }
            }
            else if (!dto.EsPagoMultiple && tipoPago != "MULTIPLE")
            {
                // Para pagos simples, crear un solo registro de método de pago
                var metodoPago = new MetodoPagoVenta
                {
                    TipoPago = tipoPago,
                    Monto = total,
                    Estado = "COMPLETADO",
                    FechaPago = DateTime.Now,
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };
                venta.MetodosPago.Add(metodoPago);
            }

            // Guardar venta y detalles
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();

            // Cargar los datos completos para retornar
            await _context.Entry(venta).Reference(v => v.Cliente).LoadAsync();
            await _context.Entry(venta).Reference(v => v.Entrega).LoadAsync();
            await _context.Entry(venta).Collection(v => v.Detalles).LoadAsync();
            foreach (var det in venta.Detalles)
            {
                await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
            }

            var cantidadTotalResponse = venta.Detalles.Sum(d => d.Cantidad);
            var totalVenta = venta.Detalles.Sum(d => d.Cantidad * d.PrecioVenta);

            var ventaDto = new VentaCompletaDto
            {
                IdVenta = venta.IdVenta,
                IdUsuario = venta.IdUsuario,
                IdCliente = venta.IdCliente,
                ClienteNombre = venta.Cliente != null ? venta.Cliente.NombreCompleto : null,
                TipoComprobante = venta.TipoComprobante,
                Cantidad = cantidadTotalResponse,
                Total = totalVenta,
                ImporteRecibido = venta.ImporteRecibido,
                ImporteCambio = venta.ImporteRecibido - totalVenta,
                TipoPago = venta.TipoPago,
                EsPagoMultiple = venta.EsPagoMultiple,
                Estado_id = venta.Estado_id,
                Fecha = venta.Fecha,
                TipoEntrega = venta.Entrega != null ? venta.Entrega.TipoEntrega : null,
                DireccionEntrega = venta.Entrega != null ? venta.Entrega.DireccionEntrega : null,
                CostoEnvio = venta.Entrega != null ? venta.Entrega.CostoEnvio : null,
                FechaEntregaProgramada = venta.Entrega != null ? venta.Entrega.FechaEntregaProgramada : null,
                EstadoEntrega = venta.Entrega != null ? venta.Entrega.EstadoEntrega : null,
                IdEntrega = venta.IdEntrega,
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

            return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta }, ventaDto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // PUT: api/Ventas/5/anular
    [HttpPut("{id}/anular")]
    public async Task<IActionResult> AnularVenta(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            if (venta.Estado_id == 0)
            {
                return BadRequest("La venta ya está anulada");
            }

            // Restaurar stock de cada producto
            foreach (var detalle in venta.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                if (producto != null)
                {
                    producto.Stock += detalle.Cantidad;
                    _context.Entry(producto).State = EntityState.Modified;
                }
            }

            // Cambiar estado de la venta a anulada
            venta.Estado_id = 0;
            _context.Entry(venta).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // PUT: api/Ventas/{id}/confirmar-pago-qr
    [HttpPut("{id}/confirmar-pago-qr")]
    public async Task<ActionResult<VentaDto>> ConfirmarPagoQR(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            if (venta.TipoPago != "QR")
            {
                return BadRequest("Esta venta no es un pago QR");
            }

            if (venta.Estado_id == 2)
            {
                return BadRequest("El pago ya está confirmado");
            }

            // Descontar stock de cada producto
            foreach (var detalle in venta.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                if (producto != null)
                {
                    if (producto.Stock < detalle.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente para el producto {producto.Descripcion}");
                    }
                    producto.Stock -= detalle.Cantidad;
                    _context.Entry(producto).State = EntityState.Modified;
                }
            }

            // Cambiar estado de la venta a pagada
            venta.Estado_id = 2;
            _context.Entry(venta).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Recargar con relaciones
            await _context.Entry(venta).Reference(v => v.Cliente).LoadAsync();
            await _context.Entry(venta).Reference(v => v.Entrega).LoadAsync();
            await _context.Entry(venta).Collection(v => v.Detalles).LoadAsync();
            foreach (var det in venta.Detalles)
            {
                await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
            }

            var cantidadTotal = venta.Detalles.Sum(d => d.Cantidad);
            var totalVenta = venta.Detalles.Sum(d => d.Cantidad * d.PrecioVenta);

            var ventaDto = new VentaCompletaDto
            {
                IdVenta = venta.IdVenta,
                IdUsuario = venta.IdUsuario,
                IdCliente = venta.IdCliente,
                ClienteNombre = venta.Cliente != null ? venta.Cliente.NombreCompleto : null,
                TipoComprobante = venta.TipoComprobante,
                Cantidad = cantidadTotal,
                Total = totalVenta,
                ImporteRecibido = totalVenta,
                ImporteCambio = 0,
                TipoPago = venta.TipoPago,
                Estado_id = venta.Estado_id,
                Fecha = venta.Fecha,
                TipoEntrega = venta.Entrega != null ? venta.Entrega.TipoEntrega : null,
                DireccionEntrega = venta.Entrega != null ? venta.Entrega.DireccionEntrega : null,
                CostoEnvio = venta.Entrega != null ? venta.Entrega.CostoEnvio : null,
                FechaEntregaProgramada = venta.Entrega != null ? venta.Entrega.FechaEntregaProgramada : null,
                EstadoEntrega = venta.Entrega != null ? venta.Entrega.EstadoEntrega : null,
                IdEntrega = venta.IdEntrega,
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

            return Ok(ventaDto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
