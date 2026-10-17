using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _context.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto
            {
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Estado_id = c.Estado_id,
                Fecha = c.Fecha
            })
            .ToListAsync();

        return categorias;
    }

    // GET: api/Categorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound();
        }

        var dto = new CategoriaDto
        {
            IdCategoria = categoria.IdCategoria,
            Nombre = categoria.Nombre,
            Estado_id = categoria.Estado_id,
            Fecha = categoria.Fecha
        };

        return dto;
    }

    // PUT: api/Categorias/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategoria(int id, CreateCategoriaDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        categoria.Nombre = dto.Nombre;
        categoria.Estado_id = dto.Estado_id;

        _context.Entry(categoria).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoriaExists(id))
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

    // POST: api/Categorias
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CreateCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Estado_id = dto.Estado_id
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var categoriaDto = new CategoriaDto
        {
            IdCategoria = categoria.IdCategoria,
            Nombre = categoria.Nombre,
            Estado_id = categoria.Estado_id,
            Fecha = categoria.Fecha
        };

        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.IdCategoria }, categoriaDto);
    }

    // DELETE: api/Categorias/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        // Verificar si hay productos asociados a esta categoría
        var productosAsociados = await _context.Productos
            .AnyAsync(p => p.IdCategoria == id);

        if (productosAsociados)
        {
            return BadRequest(new { 
                message = $"No se puede eliminar la categoría '{categoria.Nombre}' porque tiene productos asociados. " +
                         $"Primero reasigne los productos a otra categoría." 
            });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoriaExists(int id)
    {
        return _context.Categorias.Any(e => e.IdCategoria == id);
    }
}
