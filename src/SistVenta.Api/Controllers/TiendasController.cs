using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiendasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TiendasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TiendaDto>>> GetTiendas()
    {
        var tiendas = await _context.Tiendas
            .OrderByDescending(t => t.IdTienda)
            .Select(t => new TiendaDto
            {
                IdTienda = t.IdTienda,
                Nombre = t.Nombre,
                Nit = t.Nit,
                Direccion = t.Direccion,
                Telefono = t.Telefono,
                Estado_id = t.Estado_id,
                Fecha = t.Fecha
            })
            .ToListAsync();

        return tiendas;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TiendaDto>> GetTienda(int id)
    {
        var tienda = await _context.Tiendas.FindAsync(id);

        if (tienda == null)
        {
            return NotFound();
        }

        var dto = new TiendaDto
        {
            IdTienda = tienda.IdTienda,
            Nombre = tienda.Nombre,
            Nit = tienda.Nit,
            Direccion = tienda.Direccion,
            Telefono = tienda.Telefono,
            Estado_id = tienda.Estado_id,
            Fecha = tienda.Fecha
        };

        return dto;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTienda(int id, CreateTiendaDto dto)
    {
        var tienda = await _context.Tiendas.FindAsync(id);
        if (tienda == null)
        {
            return NotFound();
        }

        tienda.Nombre = dto.Nombre;
        tienda.Nit = dto.Nit ?? string.Empty;
        tienda.Direccion = dto.Direccion ?? string.Empty;
        tienda.Telefono = dto.Telefono ?? string.Empty;
        tienda.Estado_id = dto.Estado_id;

        _context.Entry(tienda).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TiendaExists(id))
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

    [HttpPost]
    public async Task<ActionResult<TiendaDto>> PostTienda(CreateTiendaDto dto)
    {
        var tienda = new Tienda
        {
            Nombre = dto.Nombre,
            Nit = dto.Nit ?? string.Empty,
            Direccion = dto.Direccion ?? string.Empty,
            Telefono = dto.Telefono ?? string.Empty,
            Estado_id = dto.Estado_id
        };

        _context.Tiendas.Add(tienda);
        await _context.SaveChangesAsync();

        var tiendaDto = new TiendaDto
        {
            IdTienda = tienda.IdTienda,
            Nombre = tienda.Nombre,
            Nit = tienda.Nit,
            Direccion = tienda.Direccion,
            Telefono = tienda.Telefono,
            Estado_id = tienda.Estado_id,
            Fecha = tienda.Fecha
        };

        return CreatedAtAction(nameof(GetTienda), new { id = tienda.IdTienda }, tiendaDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTienda(int id)
    {
        var tienda = await _context.Tiendas.FindAsync(id);
        if (tienda == null)
        {
            return NotFound();
        }

        _context.Tiendas.Remove(tienda);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TiendaExists(int id)
    {
        return _context.Tiendas.Any(e => e.IdTienda == id);
    }
}
