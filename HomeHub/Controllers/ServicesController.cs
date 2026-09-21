using HomeHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;
    public ServicesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var services = await _context.Services
            .OrderBy(s => s.Name)
            .Select(s => new { id = s.Id, name = s.Name, description = s.Description })
            .ToListAsync();
        return Ok(services);
    }
}
