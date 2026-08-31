using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarSystem.Models;

[Table("Inventar")]
public class Inventar
{
    [Key]
    [Column("inventarId")]
    public int InventarId { get; set; }

    [Column("inventarItem")]
    public string Item { get; set; } = string.Empty;

    [Column("inventarSn")]
    public string? Sn { get; set; }

    [Column("inventarAntal")]
    public int Antal { get; set; }
}