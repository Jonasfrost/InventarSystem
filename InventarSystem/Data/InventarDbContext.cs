using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using InventarSystem.Models;
using InventarSystem.Services;

namespace InventarSystem.Data;

public class InventarDbContext : DbContext
{
    private readonly EncryptionService _encryptionService = new();

    public InventarDbContext(DbContextOptions<InventarDbContext> options) : base(options) { }

    public DbSet<Elev> Elev { get; set; }
    public DbSet<Instruktor> Instruktor { get; set; }
    public DbSet<Inventar> Inventar { get; set; }
    public DbSet<Udlaant> Udlaant { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ValueConverter opdateret med string? og null-tjek for at undgå CS8620
        var converter = new ValueConverter<string?, string?>(
            v => string.IsNullOrEmpty(v) ? v : _encryptionService.Encrypt(v),
            v => string.IsNullOrEmpty(v) ? v : _encryptionService.Decrypt(v)
        );

        // 1. ELEVER TABEL MAPPING
        modelBuilder.Entity<Elev>(entity =>
        {
            entity.ToTable("Elever");

            entity.HasKey(e => e.ElevId);
            entity.Property(e => e.ElevId).HasColumnName("eleverId");

            entity.Property(e => e.Fn).HasColumnName("eleverFn").HasConversion(converter).HasMaxLength(255);
            entity.Property(e => e.Ln).HasColumnName("eleverLn").HasConversion(converter).HasMaxLength(255);
            entity.Property(e => e.Mail).HasColumnName("eleverMail").HasConversion(converter).HasMaxLength(255);
            entity.Property(e => e.Mobil).HasColumnName("eleverMobil").HasConversion(converter).HasMaxLength(255);

            entity.Property(e => e.InstruktorId).HasColumnName("eleverInstruktorId");
        });

        // 2. INSTRUKTOR TABEL MAPPING (Med AES-256 kryptering)
        modelBuilder.Entity<Instruktor>(entity =>
        {
            entity.ToTable("Instruktor");

            entity.HasKey(i => i.InstruktorId);
            entity.Property(i => i.InstruktorId).HasColumnName("instruktorId");

            // Kryptering på instruktørens persondata
            entity.Property(i => i.Fn).HasColumnName("instruktorFn").HasConversion(converter).HasMaxLength(255);
            entity.Property(i => i.Ln).HasColumnName("instruktorLn").HasConversion(converter).HasMaxLength(255);
            entity.Property(i => i.Mail).HasColumnName("instruktorMail").HasConversion(converter).HasMaxLength(255);
            entity.Property(i => i.Mobil).HasColumnName("instruktorMobil").HasConversion(converter).HasMaxLength(255);
        });

        modelBuilder.Entity<Inventar>().ToTable("Inventar");
        modelBuilder.Entity<Udlaant>().ToTable("Udlaant");
    }
}