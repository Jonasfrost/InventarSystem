using Microsoft.EntityFrameworkCore;
using InventarSystem.Models;

namespace InventarSystem.Data;

public class InventarDbContext : DbContext
{
    public InventarDbContext(DbContextOptions<InventarDbContext> options) : base(options) { }

    public DbSet<Inventar> Inventar { get; set; }
    public DbSet<Udlaant> Udlaant { get; set; }
    public DbSet<Elev> Elever { get; set; }
    public DbSet<Instruktor> Instruktor { get; set; }
}