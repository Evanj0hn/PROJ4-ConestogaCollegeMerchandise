using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ConestogaCollegeMerchandise.Data;
using ConestogaCollegeMerchandise.Models;
using System.IO;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container and enable Newtonsoft JSON for JSON Patch support.
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Register the EF Core context using the connection string from appsettings.json.
builder.Services.AddDbContext<MerchandiseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger for API documentation and testing.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database with sample products
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MerchandiseContext>();
    // Ensure the database is created.
    context.Database.EnsureCreated();

    if (!context.MerchandiseItems.Any())
    {
        context.MerchandiseItems.AddRange(
            new Merchandise
            {
                Name = "Conestoga Hoodie",
                Description = "A comfortable and stylish hoodie featuring Conestoga branding.",
                Price = 39.99m,
                ImageUrl = "images/hoodie.jpg"  
            },
            new Merchandise
            {
                Name = "Conestoga T-Shirt",
                Description = "A classic t-shirt made from 100% cotton with the Conestoga logo.",
                Price = 19.99m,
                ImageUrl = "images/tshirt.jpg"  
            },
            new Merchandise
            {
                Name = "Conestoga Keychain",
                Description = "A sleek keychain featuring the Conestoga College emblem.",
                Price = 9.99m,
                ImageUrl = "images/keychain.jpg" 
            }
        );
        context.SaveChanges();
    }
}

// Serve default files (like index.html) from the "public" folder.
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "public")),
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "public")),
    RequestPath = ""
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
