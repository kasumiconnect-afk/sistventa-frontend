using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Payments/health
    [HttpGet("health")]
    [AllowAnonymous] // Permitir acceso sin autenticación
    public ActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    // POST: api/Payments/webhook
    [HttpPost("webhook")]
    [AllowAnonymous] // Permitir acceso sin autenticación para webhooks de pago
    public async Task<ActionResult<PaymentResponseDto>> ReceiveWebhook([FromBody] PaymentWebhookDto webhook)
    {
        // En producción, esto vendría de CUCU, Circle, etc.
        // Para demo, lo simulamos desde el frontend
        
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == webhook.venta_id);
            
            if (venta == null)
            {
                return NotFound(new PaymentResponseDto 
                { 
                    status = "ERROR", 
                    venta_id = webhook.venta_id, 
                    message = "Venta no encontrada" 
                });
            }

            switch (webhook.status.ToUpper())
            {
                case "PAID":
                    // Solo procesar si la venta está pendiente de pago
                    if (venta.Estado_id == 1) // PENDIENTE
                    {
                        // Descontar stock de cada producto
                        foreach (var detalle in venta.Detalles)
                        {
                            var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                            if (producto != null)
                            {
                                if (producto.Stock < detalle.Cantidad)
                                {
                                    await transaction.RollbackAsync();
                                    return BadRequest(new PaymentResponseDto 
                                    { 
                                        status = "ERROR", 
                                        venta_id = venta.IdVenta,
                                        message = $"Stock insuficiente para el producto {producto.Descripcion}" 
                                    });
                                }
                                producto.Stock -= detalle.Cantidad;
                                _context.Entry(producto).State = EntityState.Modified;
                            }
                        }

                        // Actualizar venta
                        venta.Estado_id = 2; // PAGADA
                        venta.TipoPago = "QR";

                        _context.Entry(venta).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    
                    return Ok(new PaymentResponseDto 
                    { 
                        status = "PAID", 
                        venta_id = venta.IdVenta,
                        message = "Pago procesado correctamente" 
                    });

                case "REJECTED":
                    // Opcional: marcar como rechazada o dejar pendiente
                    // Por ahora, dejamos pendiente para que el cajero pueda manejarlo
                    return Ok(new PaymentResponseDto 
                    { 
                        status = "REJECTED", 
                        venta_id = venta.IdVenta,
                        message = "Pago rechazado por el cliente" 
                    });

                case "ERROR":
                    return Ok(new PaymentResponseDto 
                    { 
                        status = "ERROR", 
                        venta_id = venta.IdVenta,
                        message = "Error en el procesamiento del pago" 
                    });

                default:
                    return BadRequest(new PaymentResponseDto 
                    { 
                        status = "ERROR", 
                        venta_id = webhook.venta_id,
                        message = "Estado de pago no reconocido" 
                    });
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // GET: api/Payments/venta/{ventaId}
    [HttpGet("venta/{ventaId}")]
    [AllowAnonymous] // Permitir acceso sin autenticación para verificar estado de pago
    public async Task<ActionResult<object>> GetPaymentStatus(int ventaId)
    {
        var venta = await _context.Ventas
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.IdVenta == ventaId);
        if (venta == null)
        {
            return NotFound();
        }

        var total = venta.Detalles.Sum(d => d.Cantidad * d.PrecioVenta);

        return new
        {
            venta_id = venta.IdVenta,
            status = venta.Estado_id == 2 ? "PAID" : "PENDING",
            tipo_pago = venta.TipoPago,
            monto = total,
            importe_recibido = venta.ImporteRecibido
        };
    }
}