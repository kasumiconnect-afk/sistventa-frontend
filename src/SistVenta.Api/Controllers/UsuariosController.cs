using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Helpers;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .OrderByDescending(u => u.IdUsuario)
            .Select(u => new UsuarioDto
            {
                IdUsuario = u.IdUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Correo = u.Correo,
                IdRol = u.IdRol,
                RolNombre = u.Rol!.Nombre,
                Estado_id = u.Estado_id,
                Fecha = u.Fecha,
                NombreCompleto = u.NombreCompleto
            })
            .ToListAsync();

        return usuarios;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario == null)
        {
            return NotFound();
        }

        var dto = new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            Correo = usuario.Correo,
            IdRol = usuario.IdRol,
            RolNombre = usuario.Rol!.Nombre,
            Estado_id = usuario.Estado_id,
            Fecha = usuario.Fecha,
            NombreCompleto = usuario.NombreCompleto
        };

        return dto;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, UpdateUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        // Verificar que el correo no exista en otro usuario
        if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo && u.IdUsuario != id))
        {
            return BadRequest("El correo ya está registrado");
        }

        usuario.Nombres = dto.Nombres;
        usuario.Apellidos = dto.Apellidos ?? string.Empty;
        usuario.Correo = dto.Correo;
        usuario.IdRol = dto.IdRol;
        usuario.Estado_id = dto.Estado_id;

        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            usuario.Contrasena = PasswordHelper.HashPassword(dto.Contrasena);
        }

        _context.Entry(usuario).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UsuarioExists(id))
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
    public async Task<ActionResult<UsuarioDto>> PostUsuario(CreateUsuarioDto dto)
    {
        // Verificar que el correo no exista
        if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
        {
            return BadRequest("El correo ya está registrado");
        }

        var usuario = new Usuario
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos ?? string.Empty,
            Correo = dto.Correo,
            Contrasena = PasswordHelper.HashPassword(dto.Contrasena),
            IdRol = dto.IdRol,
            Estado_id = dto.Estado_id
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

        var usuarioDto = new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            Correo = usuario.Correo,
            IdRol = usuario.IdRol,
            RolNombre = usuario.Rol!.Nombre,
            Estado_id = usuario.Estado_id,
            Fecha = usuario.Fecha,
            NombreCompleto = usuario.NombreCompleto
        };

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuarioDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.IdUsuario == id);
    }
}
