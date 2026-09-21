namespace HomeHub.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public string Description { get; set; } = "";
    public string ServiceAddress { get; set; } = "";
    public DateTime PreferredDate { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Amount { get; set; }
}
