using HomeHub.Data;
using HomeHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _context;
    public PaymentsController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentRequest request)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(request.ServiceRequestId);
        if (serviceRequest == null) return NotFound(new { message = "Service request not found." });
        if (serviceRequest.Status != "Completed") return BadRequest(new { message = "Payment is only available after the job is completed." });

        var method = request.PaymentMethod.Trim();
        if (!method.Equals("Cash", StringComparison.OrdinalIgnoreCase) &&
            !method.Equals("Card", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Payment method must be Cash or Card." });

        if (await _context.Payments.AnyAsync(p => p.ServiceRequestId == request.ServiceRequestId))
            return Conflict(new { message = "Payment has already been confirmed for this request." });

        var payment = new Payment
        {
            ServiceRequestId = serviceRequest.Id,
            Amount = serviceRequest.Amount,
            PaymentMethod = method,
            PaymentStatus = "Paid",
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Payment method saved successfully",
            paymentId = payment.Id,
            amount = payment.Amount,
            paymentMethod = payment.PaymentMethod,
            paymentStatus = payment.PaymentStatus
        });
    }
}
