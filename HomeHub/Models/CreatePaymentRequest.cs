namespace HomeHub.Models;

public class CreatePaymentRequest
{
    public int ServiceRequestId { get; set; }
    public string PaymentMethod { get; set; } = "";
}
