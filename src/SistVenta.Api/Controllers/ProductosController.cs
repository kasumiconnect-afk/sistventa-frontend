using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Productos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .OrderByDescending(p => p.IdProducto)
            .Select(p => new ProductoDto
            {
                IdProducto = p.IdProducto,
                CodigoSKU = p.CodigoSKU,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                Modelo = p.Modelo,
                Presentacion = p.Presentacion,
                UnidadMedida = p.UnidadMedida,
                Stock = p.Stock,
                StockMinimo = p.StockMinimo,
                UbicacionAlmacen = p.UbicacionAlmacen,
                PrecioCompra = p.PrecioCompra,
                Costopromedio = p.Costopromedio,
                MargenPorcentaje = p.MargenPorcentaje,
                PrecioVenta = p.PrecioVenta,
                PrecioMayorista = p.PrecioMayorista,
                IdCategoria = p.IdCategoria,
                CategoriaNombre = p.Categoria!.Nombre,
                Estado_id = p.Estado_id,
                Fecha = p.Fecha,
                NombreCompleto = p.NombreCompleto,
                StockBajo = p.StockBajo
            })
            .ToListAsync();

        return productos;
    }

    // GET: api/Productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> GetProducto(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null)
        {
            return NotFound();
        }

        var dto = new ProductoDto
        {
            IdProducto = producto.IdProducto,
            CodigoSKU = producto.CodigoSKU,
            Descripcion = producto.Descripcion,
            Marca = producto.Marca,
            Modelo = producto.Modelo,
            Presentacion = producto.Presentacion,
            UnidadMedida = producto.UnidadMedida,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            UbicacionAlmacen = producto.UbicacionAlmacen,
            PrecioCompra = producto.PrecioCompra,
            Costopromedio = producto.Costopromedio,
            MargenPorcentaje = producto.MargenPorcentaje,
            PrecioVenta = producto.PrecioVenta,
            PrecioMayorista = producto.PrecioMayorista,
            IdCategoria = producto.IdCategoria,
            CategoriaNombre = producto.Categoria!.Nombre,
            Estado_id = producto.Estado_id,
            Fecha = producto.Fecha,
            NombreCompleto = producto.NombreCompleto,
            StockBajo = producto.StockBajo
        };

        return dto;
    }

    // PUT: api/Productos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProducto(int id, CreateProductoDto dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound();
        }

        // Validar unicidad del SKU (excluyendo el producto actual)
        var skuExistente = await _context.Productos
            .AnyAsync(p => p.CodigoSKU == dto.CodigoSKU.ToUpper().Trim() && p.IdProducto != id);

        if (skuExistente)
        {
            return BadRequest(new { message = "El Código SKU ya existe en otro producto." });
        }

        producto.CodigoSKU = dto.CodigoSKU.ToUpper().Trim();
        producto.Descripcion = dto.Descripcion;
        producto.Marca = dto.Marca ?? "Genérico";
        producto.Modelo = dto.Modelo ?? string.Empty;
        producto.Presentacion = dto.Presentacion ?? "Unidad";
        producto.UnidadMedida = dto.UnidadMedida ?? "Unidad";
        producto.Stock = dto.Stock;
        producto.StockMinimo = dto.StockMinimo ?? 5;
        producto.UbicacionAlmacen = dto.UbicacionAlmacen ?? string.Empty;
        producto.PrecioCompra = dto.PrecioCompra;
        producto.Costopromedio = dto.Costopromedio;
        producto.MargenPorcentaje = dto.MargenPorcentaje;
        producto.PrecioVenta = CalcularPrecioVenta(dto.Costopromedio, dto.MargenPorcentaje);
        producto.PrecioMayorista = dto.PrecioMayorista > 0
            ? dto.PrecioMayorista
            : CalcularPrecioVenta(dto.Costopromedio, dto.MargenPorcentaje - 5);
        producto.IdCategoria = dto.IdCategoria;
        producto.Estado_id = dto.Estado_id;

        _context.Entry(producto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Productos
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> PostProducto(CreateProductoDto dto)
    {
        // Validar unicidad del SKU
        var skuExistente = await _context.Productos
            .AnyAsync(p => p.CodigoSKU == dto.CodigoSKU.ToUpper().Trim());

        if (skuExistente)
        {
            return BadRequest(new { message = "El Código SKU ya existe. Por favor, usa un código único." });
        }

        var producto = new Producto
        {
            CodigoSKU = dto.CodigoSKU.ToUpper().Trim(),
            Descripcion = dto.Descripcion,
            Marca = dto.Marca ?? "Genérico",
            Modelo = dto.Modelo ?? string.Empty,
            Presentacion = dto.Presentacion ?? "Unidad",
            UnidadMedida = dto.UnidadMedida ?? "Unidad",
            Stock = dto.Stock,
            StockMinimo = dto.StockMinimo ?? 5,
            UbicacionAlmacen = dto.UbicacionAlmacen ?? string.Empty,
            PrecioCompra = dto.PrecioCompra,
            Costopromedio = dto.Costopromedio,
            MargenPorcentaje = dto.MargenPorcentaje,
            PrecioVenta = CalcularPrecioVenta(dto.Costopromedio, dto.MargenPorcentaje),
            PrecioMayorista = dto.PrecioMayorista > 0
                ? dto.PrecioMayorista
                : CalcularPrecioVenta(dto.Costopromedio, dto.MargenPorcentaje - 5),
            IdCategoria = dto.IdCategoria,
            Estado_id = dto.Estado_id
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Cargar la categoría para el DTO
        await _context.Entry(producto).Reference(p => p.Categoria).LoadAsync();

        var productoDto = new ProductoDto
        {
            IdProducto = producto.IdProducto,
            CodigoSKU = producto.CodigoSKU,
            Descripcion = producto.Descripcion,
            Marca = producto.Marca,
            Modelo = producto.Modelo,
            Presentacion = producto.Presentacion,
            UnidadMedida = producto.UnidadMedida,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            UbicacionAlmacen = producto.UbicacionAlmacen,
            PrecioCompra = producto.PrecioCompra,
            Costopromedio = producto.Costopromedio,
            MargenPorcentaje = producto.MargenPorcentaje,
            PrecioVenta = producto.PrecioVenta,
            PrecioMayorista = producto.PrecioMayorista,
            IdCategoria = producto.IdCategoria,
            CategoriaNombre = producto.Categoria!.Nombre,
            Estado_id = producto.Estado_id,
            Fecha = producto.Fecha,
            NombreCompleto = producto.NombreCompleto,
            StockBajo = producto.StockBajo
        };

        return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, productoDto);
    }

    // DELETE: api/Productos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound();
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductoExists(int id)
    {
        return _context.Productos.Any(e => e.IdProducto == id);
    }

    private static decimal CalcularPrecioVenta(decimal costoPromedio, decimal margenPorcentaje)
    {
        return Math.Round(costoPromedio * (1 + margenPorcentaje / 100m), 2);
    }
}
