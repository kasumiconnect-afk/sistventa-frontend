using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.Models;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntregasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EntregasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Entregas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entrega>>> GetEntregas()
    {
        return await _context.Entregas.ToListAsync();
    }

    // GET: api/Entregas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Entrega>> GetEntrega(int id)
    {
        var entrega = await _context.Entregas.FindAsync(id);

        if (entrega == null)
        {
            return NotFound();
        }

        return entrega;
    }

    // POST: api/Entregas
    [HttpPost]
    public async Task<ActionResult<Entrega>> CreateEntrega([FromBody] Entrega entrega)
    {
        entrega.Fecha = DateTime.UtcNow;
        if (entrega.Estado_id == null)
        {
            entrega.Estado_id = 1; // Activo por defecto
        }

        _context.Entregas.Add(entrega);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEntrega), new { id = entrega.IdEntrega }, entrega);
    }

    // PUT: api/Entregas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEntrega(int id, [FromBody] Entrega entrega)
    {
        if (id != entrega.IdEntrega)
        {
            return BadRequest();
        }

        _context.Entry(entrega).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EntregaExists(id))
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

    // DELETE: api/Entregas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntrega(int id)
    {
        var entrega = await _context.Entregas.FindAsync(id);
        if (entrega == null)
        {
            return NotFound();
        }

        _context.Entregas.Remove(entrega);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EntregaExists(int id)
    {
        return _context.Entregas.Any(e => e.IdEntrega == id);
    }
}
