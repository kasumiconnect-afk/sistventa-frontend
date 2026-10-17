using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermisosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PermisosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermisoDto>>> GetPermisos()
    {
        var permisos = await _context.Permisos
            .Include(p => p.Modulo)
            .Where(p => p.Estado_id == 1)
            .Select(p => new PermisoDto
            {
                IdPermiso = p.IdPermiso,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                IdModulo = p.IdModulo,
                ModuloNombre = p.Modulo != null ? p.Modulo.Nombre : string.Empty,
                Estado_id = p.Estado_id,
                Fecha = p.Fecha
            })
            .ToListAsync();

        return permisos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PermisoDto>> GetPermiso(int id)
    {
        var permiso = await _context.Permisos
            .Include(p => p.Modulo)
            .FirstOrDefaultAsync(p => p.IdPermiso == id);

        if (permiso == null) return NotFound();

        var permisoDto = new PermisoDto
        {
            IdPermiso = permiso.IdPermiso,
            Codigo = permiso.Codigo,
            Descripcion = permiso.Descripcion,
            IdModulo = permiso.IdModulo,
            ModuloNombre = permiso.Modulo != null ? permiso.Modulo.Nombre : string.Empty,
            Estado_id = permiso.Estado_id,
            Fecha = permiso.Fecha
        };

        return permisoDto;
    }

    [HttpPost]
    public async Task<ActionResult<PermisoDto>> PostPermiso(CreatePermisoDto dto)
    {
        var permiso = new Permiso
        {
            Codigo = dto.Codigo,
            Descripcion = dto.Descripcion,
            IdModulo = dto.IdModulo,
            Estado_id = 1,
            Fecha = DateTime.Now
        };

        _context.Permisos.Add(permiso);
        await _context.SaveChangesAsync();

        await _context.Entry(permiso).Reference(p => p.Modulo).LoadAsync();

        var permisoDto = new PermisoDto
        {
            IdPermiso = permiso.IdPermiso,
            Codigo = permiso.Codigo,
            Descripcion = permiso.Descripcion,
            IdModulo = permiso.IdModulo,
            ModuloNombre = permiso.Modulo != null ? permiso.Modulo.Nombre : string.Empty,
            Estado_id = permiso.Estado_id,
            Fecha = permiso.Fecha
        };

        return CreatedAtAction(nameof(GetPermiso), new { id = permiso.IdPermiso }, permisoDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPermiso(int id, UpdatePermisoDto dto)
    {
        var permiso = await _context.Permisos.FindAsync(id);
        if (permiso == null) return NotFound();

        permiso.Codigo = dto.Codigo;
        permiso.Descripcion = dto.Descripcion;
        permiso.IdModulo = dto.IdModulo;

        _context.Entry(permiso).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermiso(int id)
    {
        var permiso = await _context.Permisos.FindAsync(id);
        if (permiso == null) return NotFound();

        permiso.Estado_id = 0;
        _context.Entry(permiso).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
