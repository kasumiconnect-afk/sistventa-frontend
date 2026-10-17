using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposComprobanteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TiposComprobanteController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoComprobanteDto>>> GetTiposComprobante()
    {
        var tipos = await _context.TiposComprobante
            .OrderBy(t => t.IdTipoComprobante)
            .Select(t => new TipoComprobanteDto
            {
                IdTipoComprobante = t.IdTipoComprobante,
                Nombre = t.Nombre,
                Estado_id = t.Estado_id
            })
            .ToListAsync();

        return tipos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoComprobanteDto>> GetTipoComprobante(int id)
    {
        var tipo = await _context.TiposComprobante.FindAsync(id);

        if (tipo == null)
        {
            return NotFound();
        }

        var dto = new TipoComprobanteDto
        {
            IdTipoComprobante = tipo.IdTipoComprobante,
            Nombre = tipo.Nombre,
            Estado_id = tipo.Estado_id
        };

        return dto;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTipoComprobante(int id, CreateTipoComprobanteDto dto)
    {
        var tipo = await _context.TiposComprobante.FindAsync(id);
        if (tipo == null)
        {
            return NotFound();
        }

        tipo.Nombre = dto.Nombre;
        tipo.Estado_id = dto.Estado_id;

        _context.Entry(tipo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TipoComprobanteExists(id))
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
    public async Task<ActionResult<TipoComprobanteDto>> PostTipoComprobante(CreateTipoComprobanteDto dto)
    {
        var tipo = new TipoComprobante
        {
            Nombre = dto.Nombre,
            Estado_id = dto.Estado_id
        };

        _context.TiposComprobante.Add(tipo);
        await _context.SaveChangesAsync();

        var tipoDto = new TipoComprobanteDto
        {
            IdTipoComprobante = tipo.IdTipoComprobante,
            Nombre = tipo.Nombre,
            Estado_id = tipo.Estado_id
        };

        return CreatedAtAction(nameof(GetTipoComprobante), new { id = tipo.IdTipoComprobante }, tipoDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTipoComprobante(int id)
    {
        var tipo = await _context.TiposComprobante.FindAsync(id);
        if (tipo == null)
        {
            return NotFound();
        }

        // Verificar si hay ventas asociadas a este tipo de comprobante
        var ventasAsociadas = await _context.Ventas
            .AnyAsync(v => v.TipoComprobante == id);

        if (ventasAsociadas)
        {
            return BadRequest(new { 
                message = $"No se puede eliminar el tipo de comprobante '{tipo.Nombre}' porque hay ventas asociadas. " +
                         $"Primero elimine o modifique las ventas que usan este tipo de comprobante." 
            });
        }

        _context.TiposComprobante.Remove(tipo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TipoComprobanteExists(int id)
    {
        return _context.TiposComprobante.Any(e => e.IdTipoComprobante == id);
    }
}
