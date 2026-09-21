using HomeHub.Data;
using HomeHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RequestsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RequestsController> _logger;

    public RequestsController(AppDbContext context, ILogger<RequestsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceRequestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.ServiceAddress))
            return BadRequest(new { message = "Description and service address are required." });

        if (request.PreferredDate.Date < DateTime.Today)
            return BadRequest(new { message = "Preferred date cannot be in the past." });

        if (!await _context.Users.AnyAsync(u => u.Id == request.UserId))
            return BadRequest(new { message = "User was not found." });

        if (!await _context.Services.AnyAsync(s => s.Id == request.ServiceId))
            return BadRequest(new { message = "Service was not found." });

        var entity = new ServiceRequest
        {
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            Description = request.Description.Trim(),
            ServiceAddress = request.ServiceAddress.Trim(),
            PreferredDate = request.PreferredDate,
            Status = "Pending",
            Amount = 0
        };

        _context.ServiceRequests.Add(entity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Service request {RequestId} created for user {UserId}", entity.Id, entity.UserId);

        return Ok(new { message = "Service request created", requestId = entity.Id, status = entity.Status });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetForUser(int userId)
    {
        var requests = await (
            from r in _context.ServiceRequests
            join s in _context.Services on r.ServiceId equals s.Id
            where r.UserId == userId
            orderby r.Id descending
            select new
            {
                id = r.Id,
                serviceId = r.ServiceId,
                serviceName = s.Name,
                description = r.Description,
                serviceAddress = r.ServiceAddress,
                preferredDate = r.PreferredDate,
                status = r.Status,
                amount = r.Amount,
                isPaid = _context.Payments.Any(p => p.ServiceRequestId == r.Id)
            }).ToListAsync();

        return Ok(requests);
    }

    // Prototype helper so the status flow can be demonstrated in Part 2.
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateRequestStatusRequest request)
    {
        var allowed = new[] { "Pending", "In Progress", "Completed" };
        if (!allowed.Contains(request.Status))
            return BadRequest(new { message = "Status must be Pending, In Progress or Completed." });

        var entity = await _context.ServiceRequests.FindAsync(id);
        if (entity == null) return NotFound(new { message = "Request not found." });

        entity.Status = request.Status;
        if (request.Amount.HasValue && request.Amount.Value >= 0)
            entity.Amount = request.Amount.Value;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Request status updated", status = entity.Status, amount = entity.Amount });
    }
}
