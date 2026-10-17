namespace SistVenta.Api.DTOs;

public class PaymentWebhookDto
{
    public string payment_id { get; set; } = string.Empty;
    public int venta_id { get; set; }
    public decimal amount { get; set; }
    public string status { get; set; } = string.Empty; // PAID, REJECTED, ERROR, PENDING
    public DateTime? timestamp { get; set; }
}

public class PaymentResponseDto
{
    public string status { get; set; } = string.Empty;
    public int venta_id { get; set; }
    public string message { get; set; } = string.Empty;
}