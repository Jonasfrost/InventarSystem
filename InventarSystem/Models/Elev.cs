using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarSystem.Models;

[Table("Elever")]
public class Elev
{
    [Key]
    [Column("eleverId")]
    public int ElevId { get; set; }

    [Column("eleverFn")]
    public string Fn { get; set; } = string.Empty;

    [Column("eleverLn")]
    public string Ln { get; set; } = string.Empty;

    [Column("eleverMail")]
    public string? Mail { get; set; }

    [Column("eleverMobil")]
    public string? Mobil { get; set; }

    // Rettes til det præcise SQL-kolonnenavn 'eleverInstruktorId'
    [Column("eleverInstruktorId")]
    public int? InstruktorId { get; set; }
}