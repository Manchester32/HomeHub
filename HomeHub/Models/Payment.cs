namespace HomeHub.Models;

public class Payment
{
    public int Id { get; set; }
    public int ServiceRequestId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string PaymentStatus { get; set; } = "Paid";
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
