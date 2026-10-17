using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProveedoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Proveedores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
    {
        var proveedores = await _context.Proveedores
            .OrderByDescending(p => p.IdProveedor)
            .Select(p => new ProveedorDto
            {
                IdProveedor = p.IdProveedor,
                Nombre = p.Nombre,
                Contacto = p.Contacto,
                Celular = p.Celular,
                Correo = p.Correo,
                Direccion = p.Direccion,
                Estado_id = p.Estado_id,
                Fecha = p.Fecha
            })
            .ToListAsync();

        return proveedores;
    }

    // GET: api/Proveedores/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorDto>> GetProveedor(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);

        if (proveedor == null)
        {
            return NotFound();
        }

        var dto = new ProveedorDto
        {
            IdProveedor = proveedor.IdProveedor,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Celular = proveedor.Celular,
            Correo = proveedor.Correo,
            Direccion = proveedor.Direccion,
            Estado_id = proveedor.Estado_id,
            Fecha = proveedor.Fecha
        };

        return dto;
    }

    // PUT: api/Proveedores/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProveedor(int id, CreateProveedorDto dto)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }

        proveedor.Nombre = dto.Nombre;
        proveedor.Contacto = dto.Contacto ?? string.Empty;
        proveedor.Celular = dto.Celular ?? string.Empty;
        proveedor.Correo = dto.Correo ?? string.Empty;
        proveedor.Direccion = dto.Direccion ?? string.Empty;
        proveedor.Estado_id = dto.Estado_id;

        _context.Entry(proveedor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProveedorExists(id))
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

    // POST: api/Proveedores
    [HttpPost]
    public async Task<ActionResult<ProveedorDto>> PostProveedor(CreateProveedorDto dto)
    {
        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto ?? string.Empty,
            Celular = dto.Celular ?? string.Empty,
            Correo = dto.Correo ?? string.Empty,
            Direccion = dto.Direccion ?? string.Empty,
            Estado_id = dto.Estado_id
        };

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();

        var proveedorDto = new ProveedorDto
        {
            IdProveedor = proveedor.IdProveedor,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Celular = proveedor.Celular,
            Correo = proveedor.Correo,
            Direccion = proveedor.Direccion,
            Estado_id = proveedor.Estado_id,
            Fecha = proveedor.Fecha
        };

        return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.IdProveedor }, proveedorDto);
    }

    // DELETE: api/Proveedores/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProveedor(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }

        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProveedorExists(int id)
    {
        return _context.Proveedores.Any(e => e.IdProveedor == id);
    }
}
