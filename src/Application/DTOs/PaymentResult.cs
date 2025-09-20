namespace Application.DTOs;

public class PaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
