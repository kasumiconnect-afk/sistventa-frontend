using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
    {
        var roles = await _context.Roles
            .OrderByDescending(r => r.IdRol)
            .Select(r => new RolDto
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Estado_id = r.Estado_id,
                Fecha = r.Fecha
            })
            .ToListAsync();

        return roles;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RolDto>> GetRol(int id)
    {
        var rol = await _context.Roles.FindAsync(id);

        if (rol == null)
        {
            return NotFound();
        }

        var dto = new RolDto
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre,
            Estado_id = rol.Estado_id,
            Fecha = rol.Fecha
        };

        return dto;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRol(int id, CreateRolDto dto)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
        {
            return NotFound();
        }

        // Verificar que el nombre no exista en otro rol
        if (await _context.Roles.AnyAsync(r => r.Nombre == dto.Nombre && r.IdRol != id))
        {
            return BadRequest("El nombre del rol ya existe");
        }

        rol.Nombre = dto.Nombre;
        rol.Estado_id = dto.Estado_id;

        _context.Entry(rol).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RolExists(id))
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
    public async Task<ActionResult<RolDto>> PostRol(CreateRolDto dto)
    {
        // Verificar que el nombre no exista
        if (await _context.Roles.AnyAsync(r => r.Nombre == dto.Nombre))
        {
            return BadRequest("El nombre del rol ya existe");
        }

        var rol = new Rol
        {
            Nombre = dto.Nombre,
            Estado_id = dto.Estado_id
        };

        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();

        var rolDto = new RolDto
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre,
            Estado_id = rol.Estado_id,
            Fecha = rol.Fecha
        };

        return CreatedAtAction(nameof(GetRol), new { id = rol.IdRol }, rolDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRol(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
        {
            return NotFound();
        }

        _context.Roles.Remove(rol);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RolExists(int id)
    {
        return _context.Roles.Any(e => e.IdRol == id);
    }
}
