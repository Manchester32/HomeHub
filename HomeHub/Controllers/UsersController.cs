using HomeHub.Data;
using HomeHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new { id = u.Id, name = u.Name, email = u.Email, homeAddress = u.HomeAddress })
            .FirstOrDefaultAsync();
        return user == null ? NotFound(new { message = "User not found." }) : Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found." });
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest(new { message = "Name is required." });

        user.Name = request.Name.Trim();
        user.HomeAddress = request.HomeAddress.Trim();
        await _context.SaveChangesAsync();
        return Ok(new { message = "Profile updated", id = user.Id, name = user.Name, email = user.Email, homeAddress = user.HomeAddress });
    }
}
