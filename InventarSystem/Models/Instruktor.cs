using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarSystem.Models;

[Table("Instruktor")]
public class Instruktor
{
    [Key]
    [Column("instruktorId")]
    public int InstruktorId { get; set; }

    [Column("instruktorFn")]
    public string Fn { get; set; } = string.Empty;

    [Column("instruktorLn")]
    public string Ln { get; set; } = string.Empty;

    [Column("instruktorMail")]
    public string? Mail { get; set; }

    [Column("instruktorMobil")]
    public string? Mobil { get; set; }

    public ICollection<Elev> Elever { get; set; } = new List<Elev>();
}