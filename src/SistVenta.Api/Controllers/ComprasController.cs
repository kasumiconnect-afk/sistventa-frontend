using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;
using Microsoft.Extensions.Logging;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComprasController> _logger;

    public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompraCompletaDto>>> GetCompras(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] int? idProveedor = null)
    {
        var inicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
        var fin = fechaFin ?? DateTime.Today;

        var query = _context.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles) // Cargar los detalles de la compra
                .ThenInclude(d => d.Producto) // Cargar los productos dentro de los detalles
            .Where(c => c.Fecha.Date >= inicio.Date && c.Fecha.Date <= fin.Date);

        if (idProveedor.HasValue)
        {
            query = query.Where(c => c.IdProveedor == idProveedor.Value);
        }

        var compras = await query
            .OrderByDescending(c => c.IdCompra)
            .Select(c => new CompraCompletaDto
            {
                IdCompra = c.IdCompra,
                IdUsuario = c.IdUsuario,
                IdProveedor = c.IdProveedor,
                ProveedorNombre = c.Proveedor != null ? c.Proveedor.Nombre : null,
                Total = c.Detalles.Sum(d => d.Cantidad * d.PrecioCompra),
                Estado_id = c.Estado_id,
                Fecha = c.Fecha,
                Detalles = c.Detalles.Select(d => new DetalleCompraCompletaDto
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto!.CodigoSKU, // Asumiendo que Producto no será null aquí
                    ProductoDescripcion = d.Producto.Descripcion,
                    Marca = d.Producto.Marca,
                    UnidadMedida = d.Producto.UnidadMedida,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioCompra,
                    Total = d.Cantidad * d.PrecioCompra
                }).ToList()
            })
            .ToListAsync();

        return compras;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompraCompletaDto>> GetCompra(int id)
    {
        var compra = await _context.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(c => c.IdCompra == id);

        if (compra == null)
        {
            return NotFound();
        }

        var compraDto = new CompraCompletaDto
        {
            IdCompra = compra.IdCompra,
            IdUsuario = compra.IdUsuario,
            IdProveedor = compra.IdProveedor,
            ProveedorNombre = compra.Proveedor != null ? compra.Proveedor.Nombre : null,
            Total = compra.Detalles.Sum(d => d.Cantidad * d.PrecioCompra),
            Estado_id = compra.Estado_id,
            Fecha = compra.Fecha,
            Detalles = compra.Detalles.Select(d => new DetalleCompraCompletaDto
            {
                IdDetalleCompra = d.IdDetalleCompra,
                IdProducto = d.IdProducto,
                CodigoSKU = d.Producto!.CodigoSKU,
                ProductoDescripcion = d.Producto.Descripcion,
                Marca = d.Producto.Marca,
                UnidadMedida = d.Producto.UnidadMedida,
                Cantidad = d.Cantidad,
                PrecioCompra = d.PrecioCompra,
                Total = d.Cantidad * d.PrecioCompra
            }).ToList()
        };

        return compraDto;
    }

    [HttpPost]
    public async Task<ActionResult<CompraDto>> PostCompra(CreateCompraDto dto)
    {
        if (dto.Detalles == null || dto.Detalles.Count == 0)
        {
            return BadRequest("La compra debe tener al menos un detalle");
        }

        // Manejar el caso cuando no se proporciona usuario
        int? idUsuarioFinal = dto.IdUsuario;
        if (!idUsuarioFinal.HasValue)
        {
            // Buscar un usuario por defecto (el primero activo) o crear uno temporal
            var usuarioDefault = await _context.Usuarios
                .Where(u => u.Estado_id == 1)
                .FirstOrDefaultAsync();
            
            if (usuarioDefault != null)
            {
                idUsuarioFinal = usuarioDefault.IdUsuario;
            }
            else
            {
                // Si no hay usuarios, crear uno temporal
                var usuarioTemp = new Usuario
                {
                    Nombres = "Usuario Temporal",
                    Apellidos = "Sistema",
                    Correo = "temporal@sistema.com",
                    Contrasena = "temp123", // En producción esto debería ser manejado de forma diferente
                    IdRol = 1, // Asumiendo que existe un rol con ID 1
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };
                _context.Usuarios.Add(usuarioTemp);
                await _context.SaveChangesAsync();
                idUsuarioFinal = usuarioTemp.IdUsuario;
            }
        }
        else
        {
            // Validar que el usuario existe si se proporciona
            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario.Value);
            if (usuario == null)
            {
                return BadRequest($"Usuario con ID {dto.IdUsuario} no encontrado");
            }
        }

        // Validar que el proveedor existe
        var proveedor = await _context.Proveedores.FindAsync(dto.IdProveedor);
        if (proveedor == null)
        {
            return BadRequest($"Proveedor con ID {dto.IdProveedor} no encontrado");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            decimal total = 0;
            var compra = new Compra
            {
                IdUsuario = idUsuarioFinal,
                IdProveedor = dto.IdProveedor,
                Estado_id = 1,
                Fecha = DateTime.Now,
                Detalles = new List<DetalleCompra>()
            };

            foreach (var detDto in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detDto.IdProducto);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {detDto.IdProducto} no encontrado");
                }

                var precioCompra = detDto.PrecioCompra > 0 ? detDto.PrecioCompra : producto.PrecioCompra;
                var subtotal = Math.Round(precioCompra * detDto.Cantidad, 2);

                var detalle = new DetalleCompra
                {
                    IdProducto = detDto.IdProducto,
                    Cantidad = detDto.Cantidad,
                    PrecioCompra = precioCompra,
                    Estado_id = 1,
                    Fecha = DateTime.Now
                };

                compra.Detalles.Add(detalle);
                total += subtotal;

                // No se asigna Total al model porque ya no existe en 3NF
                int stockAnterior = producto.Stock;
                decimal costoPromedioAnterior = producto.Costopromedio;
                int nuevoStock = stockAnterior + detDto.Cantidad;
                decimal nuevoCostoPromedio = nuevoStock > 0 
                    ? Math.Round(((stockAnterior * costoPromedioAnterior) + (detDto.Cantidad * precioCompra)) / nuevoStock, 2)
                    : precioCompra;

                // Actualizar producto
                producto.Stock = nuevoStock;
                producto.Costopromedio = nuevoCostoPromedio;
                producto.PrecioCompra = precioCompra;
                producto.PrecioVenta = Math.Round(nuevoCostoPromedio * (1 + producto.MargenPorcentaje / 100), 2);
                _context.Entry(producto).State = EntityState.Modified;
            }

            // No se asigna Total al model porque ya no existe en 3NF

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _context.Entry(compra).Reference(c => c.Proveedor).LoadAsync();
            await _context.Entry(compra).Collection(c => c.Detalles).LoadAsync();
            foreach (var det in compra.Detalles)
            {
                await _context.Entry(det).Reference(d => d.Producto).LoadAsync();
            }

            var compraDto = new CompraCompletaDto
            {
                IdCompra = compra.IdCompra,
                IdUsuario = compra.IdUsuario,
                IdProveedor = compra.IdProveedor,
                ProveedorNombre = compra.Proveedor != null ? compra.Proveedor.Nombre : null,
                Total = total,
                Estado_id = compra.Estado_id,
                Fecha = compra.Fecha,
                Detalles = compra.Detalles.Select(d => new DetalleCompraCompletaDto
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.IdProducto,
                    CodigoSKU = d.Producto!.CodigoSKU,
                    ProductoDescripcion = d.Producto.Descripcion,
                    Marca = d.Producto.Marca,
                    UnidadMedida = d.Producto.UnidadMedida,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioCompra,
                    Total = d.Cantidad * d.PrecioCompra
                }).ToList()
            };

            return CreatedAtAction(nameof(GetCompra), new { id = compra.IdCompra }, compraDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al guardar la compra en el controlador de compras.");
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // PUT: api/Compras/5/anular
    [HttpPut("{id}/anular")]
    public async Task<IActionResult> AnularCompra(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var compra = await _context.Compras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compra == null)
            {
                return NotFound();
            }

            if (compra.Estado_id == 0)
            {
                return BadRequest("La compra ya está anulada");
            }

            // Descontar stock y reestablecer costopromedio (utilizando lógica inversa de la compra)
            foreach (var detalle in compra.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                if (producto != null)
                {
                    // Restablecer stock
                    producto.Stock -= detalle.Cantidad;
                    
                    // Recalcular costopromedio
                    if (producto.Stock > 0)
                    {
                        // Para simplificar, volvemos a usar el precioCompra del producto base (si es que queremos una lógica más avanzada necesitaríamos más datos)
                        // En este caso, mantendremos el último valor registrado
                    }
                    
                    _context.Entry(producto).State = EntityState.Modified;
                }
            }

            // Cambiar estado de la compra a anulada
            compra.Estado_id = 0;
            _context.Entry(compra).State = EntityState.Modified;

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
}
