namespace HomeHub.Models;

public class CreateServiceRequestRequest
{
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public string Description { get; set; } = "";
    public string ServiceAddress { get; set; } = "";
    public DateTime PreferredDate { get; set; }
}
