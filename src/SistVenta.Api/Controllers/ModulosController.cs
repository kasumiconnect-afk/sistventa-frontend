using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ModulosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuloDto>>> GetModulos()
    {
        var modulos = await _context.Modulos
            .Where(m => m.Estado_id == 1)
            .OrderBy(m => m.Orden)
            .Select(m => new ModuloDto
            {
                IdModulo = m.IdModulo,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Icono = m.Icono,
                Url = m.Url,
                Orden = m.Orden,
                Estado_id = m.Estado_id,
                Fecha = m.Fecha
            })
            .ToListAsync();

        return modulos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModuloDto>> GetModulo(int id)
    {
        var modulo = await _context.Modulos.FindAsync(id);
        if (modulo == null) return NotFound();

        var moduloDto = new ModuloDto
        {
            IdModulo = modulo.IdModulo,
            Nombre = modulo.Nombre,
            Descripcion = modulo.Descripcion,
            Icono = modulo.Icono,
            Url = modulo.Url,
            Orden = modulo.Orden,
            Estado_id = modulo.Estado_id,
            Fecha = modulo.Fecha
        };

        return moduloDto;
    }

    [HttpPost]
    public async Task<ActionResult<ModuloDto>> PostModulo(CreateModuloDto dto)
    {
        var modulo = new Modulo
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Icono = dto.Icono,
            Url = dto.Url,
            Orden = dto.Orden,
            Estado_id = 1,
            Fecha = DateTime.Now
        };

        _context.Modulos.Add(modulo);
        await _context.SaveChangesAsync();

        var moduloDto = new ModuloDto
        {
            IdModulo = modulo.IdModulo,
            Nombre = modulo.Nombre,
            Descripcion = modulo.Descripcion,
            Icono = modulo.Icono,
            Url = modulo.Url,
            Orden = modulo.Orden,
            Estado_id = modulo.Estado_id,
            Fecha = modulo.Fecha
        };

        return CreatedAtAction(nameof(GetModulo), new { id = modulo.IdModulo }, moduloDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutModulo(int id, UpdateModuloDto dto)
    {
        var modulo = await _context.Modulos.FindAsync(id);
        if (modulo == null) return NotFound();

        modulo.Nombre = dto.Nombre;
        modulo.Descripcion = dto.Descripcion;
        modulo.Icono = dto.Icono;
        modulo.Url = dto.Url;
        modulo.Orden = dto.Orden;

        _context.Entry(modulo).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModulo(int id)
    {
        var modulo = await _context.Modulos.FindAsync(id);
        if (modulo == null) return NotFound();

        modulo.Estado_id = 0;
        _context.Entry(modulo).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
