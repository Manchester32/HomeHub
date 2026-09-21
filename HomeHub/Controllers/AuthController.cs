using HomeHub.Data;
using HomeHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HomeHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        request.Name = request.Name.Trim();
        request.Email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword))
            return BadRequest(new { message = "Please complete all fields." });

        if (!IsValidEmail(request.Email))
            return BadRequest(new { message = "Please enter a valid email address." });

        if (request.Password != request.ConfirmPassword)
            return BadRequest(new { message = "Passwords do not match." });

        if (!IsValidPassword(request.Password))
            return BadRequest(new { message = "Password must be at least 6 characters and contain uppercase, lowercase, number and symbol." });

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Email is already registered." });

        var user = new User { Name = request.Name, Email = request.Email };
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New HomeHub user registered with ID {UserId}", user.Id);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid email or password." });

        _logger.LogInformation("HomeHub user {UserId} logged in", user.Id);
        return Ok(new
        {
            message = "Login successful",
            userId = user.Id,
            name = user.Name,
            email = user.Email
        });
    }

    private static bool IsValidPassword(string password) =>
        password.Length >= 6 &&
        Regex.IsMatch(password, "[A-Z]") &&
        Regex.IsMatch(password, "[a-z]") &&
        Regex.IsMatch(password, "[0-9]") &&
        Regex.IsMatch(password, @"[^a-zA-Z0-9]");

    private static bool IsValidEmail(string email)
    {
        try { return new MailAddress(email).Address == email; }
        catch { return false; }
    }
}
