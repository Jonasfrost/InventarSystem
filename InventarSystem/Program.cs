using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrer Razor Pages
builder.Services.AddRazorPages();

// 2. Registrer Database Context
builder.Services.AddDbContext<InventarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 3. Konfigurer HTTP-pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// 4. Starter webserveren og holder den i gang (skal stå til sidst)
app.Run();