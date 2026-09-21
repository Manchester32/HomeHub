using HomeHub.Data;
using HomeHub.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTP is intentionally allowed during local Android/BlueStacks development.
// Use HTTPS when the API is deployed to a real host.
app.UseAuthorization();
app.MapControllers();

// Create the SQLite database automatically and add the HomeHub services.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Services.Any())
    {
        db.Services.AddRange(
            new Service { Name = "Plumbing", Description = "Plumbing repairs and maintenance" },
            new Service { Name = "Electrical", Description = "Electrical repairs and installations" },
            new Service { Name = "Cleaning", Description = "Home cleaning services" },
            new Service { Name = "Painting", Description = "Interior and exterior painting" },
            new Service { Name = "Pest Control", Description = "Household pest control" },
            new Service { Name = "Appliance Repair", Description = "Repair household appliances" },
            new Service { Name = "Carpentry", Description = "Furniture and wood repairs" },
            new Service { Name = "Garden & Landscaping", Description = "Garden and landscaping services" },
            new Service { Name = "Handyman", Description = "General home repairs" }
        );
        db.SaveChanges();
    }
}

app.Run();
