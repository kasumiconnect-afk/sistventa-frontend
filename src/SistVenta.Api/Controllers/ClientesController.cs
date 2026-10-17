using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _context.Clientes
            .OrderByDescending(c => c.IdCliente)
            .Select(c => new ClienteDto
            {
                IdCliente = c.IdCliente,
                CedulaIdentidad = c.CedulaIdentidad,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Direccion = c.Direccion,
                Telefono = c.Telefono,
                Estado_id = c.Estado_id,
                Fecha = c.Fecha,
                NombreCompleto = c.NombreCompleto
            })
            .ToListAsync();

        return clientes;
    }

    // GET: api/Clientes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        var dto = new ClienteDto
        {
            IdCliente = cliente.IdCliente,
            CedulaIdentidad = cliente.CedulaIdentidad,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Estado_id = cliente.Estado_id,
            Fecha = cliente.Fecha,
            NombreCompleto = cliente.NombreCompleto
        };

        return dto;
    }

    // PUT: api/Clientes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, CreateClienteDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        // Validar unicidad de cédula/identidad (excluyendo el cliente actual)
        var cedulaExistente = await _context.Clientes
            .AnyAsync(c => c.CedulaIdentidad == dto.CedulaIdentidad && c.IdCliente != id);

        if (cedulaExistente)
        {
            return BadRequest(new { message = "La cédula/identidad ya está registrada en otro cliente." });
        }

        cliente.CedulaIdentidad = dto.CedulaIdentidad ?? string.Empty;
        cliente.Nombres = dto.Nombres;
        cliente.Apellidos = dto.Apellidos ?? string.Empty;
        cliente.Direccion = dto.Direccion ?? string.Empty;
        cliente.Telefono = dto.Telefono ?? string.Empty;
        cliente.Estado_id = dto.Estado_id;

        _context.Entry(cliente).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClienteExists(id))
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

    // POST: api/Clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> PostCliente(CreateClienteDto dto)
    {
        // Validar unicidad de cédula/identidad
        var cedulaExistente = await _context.Clientes
            .AnyAsync(c => c.CedulaIdentidad == dto.CedulaIdentidad);

        if (cedulaExistente)
        {
            return BadRequest(new { message = "La cédula/identidad ya está registrada. Por favor, usa un documento único." });
        }

        var cliente = new Cliente
        {
            CedulaIdentidad = dto.CedulaIdentidad ?? string.Empty,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos ?? string.Empty,
            Direccion = dto.Direccion ?? string.Empty,
            Telefono = dto.Telefono ?? string.Empty,
            Estado_id = dto.Estado_id
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var clienteDto = new ClienteDto
        {
            IdCliente = cliente.IdCliente,
            CedulaIdentidad = cliente.CedulaIdentidad,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Estado_id = cliente.Estado_id,
            Fecha = cliente.Fecha,
            NombreCompleto = cliente.NombreCompleto
        };

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, clienteDto);
    }

    // DELETE: api/Clientes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClienteExists(int id)
    {
        return _context.Clientes.Any(e => e.IdCliente == id);
    }
}
