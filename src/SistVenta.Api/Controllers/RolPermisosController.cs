using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolPermisosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolPermisosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("rol/{idRol}")]
    public async Task<ActionResult<IEnumerable<RolPermisoDto>>> GetPermisosByRol(int idRol)
    {
        var rolPermisos = await _context.RolPermisos
            .Include(rp => rp.Rol)
            .Include(rp => rp.Permiso)
            .ThenInclude(p => p.Modulo)
            .Where(rp => rp.IdRol == idRol && rp.Estado_id == 1)
            .Select(rp => new RolPermisoDto
            {
                IdRolPermiso = rp.IdRolPermiso,
                IdRol = rp.IdRol,
                RolNombre = rp.Rol != null ? rp.Rol.Nombre : string.Empty,
                IdPermiso = rp.IdPermiso,
                PermisoCodigo = rp.Permiso != null ? rp.Permiso.Codigo : string.Empty,
                PermisoDescripcion = rp.Permiso != null ? rp.Permiso.Descripcion : string.Empty,
                Estado_id = rp.Estado_id,
                Fecha = rp.Fecha
            })
            .ToListAsync();

        return rolPermisos;
    }

    [HttpPost]
    public async Task<ActionResult<RolPermisoDto>> PostRolPermiso(CreateRolPermisoDto dto)
    {
        var existing = await _context.RolPermisos
            .FirstOrDefaultAsync(rp => rp.IdRol == dto.IdRol && rp.IdPermiso == dto.IdPermiso && rp.Estado_id == 1);

        if (existing != null) return BadRequest("Este permiso ya está asignado a este rol.");

        var rolPermiso = new RolPermiso
        {
            IdRol = dto.IdRol,
            IdPermiso = dto.IdPermiso,
            Estado_id = 1,
            Fecha = DateTime.Now
        };

        _context.RolPermisos.Add(rolPermiso);
        await _context.SaveChangesAsync();

        await _context.Entry(rolPermiso).Reference(rp => rp.Rol).LoadAsync();
        await _context.Entry(rolPermiso).Reference(rp => rp.Permiso).LoadAsync();

        var rolPermisoDto = new RolPermisoDto
        {
            IdRolPermiso = rolPermiso.IdRolPermiso,
            IdRol = rolPermiso.IdRol,
            RolNombre = rolPermiso.Rol != null ? rolPermiso.Rol.Nombre : string.Empty,
            IdPermiso = rolPermiso.IdPermiso,
            PermisoCodigo = rolPermiso.Permiso != null ? rolPermiso.Permiso.Codigo : string.Empty,
            PermisoDescripcion = rolPermiso.Permiso != null ? rolPermiso.Permiso.Descripcion : string.Empty,
            Estado_id = rolPermiso.Estado_id,
            Fecha = rolPermiso.Fecha
        };

        return CreatedAtAction(nameof(GetPermisosByRol), new { idRol = dto.IdRol }, rolPermisoDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRolPermiso(int id)
    {
        var rolPermiso = await _context.RolPermisos.FindAsync(id);
        if (rolPermiso == null) return NotFound();

        rolPermiso.Estado_id = 0;
        _context.Entry(rolPermiso).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
