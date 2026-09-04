using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarSystem.Models;

[Table("Udlaant")]
public class Udlaant
{
    [Key]
    [Column("udlaantId")]
    public int UdlaantId { get; set; }

    [Column("eleverId")]
    public int EleverId { get; set; }

    [Column("inventarId")]
    public int InventarId { get; set; }

    [Column("udlaantAntal")]
    public int UdlaantAntal { get; set; }

    [Column("udlaantDato")]
    public DateTime UdlaantDato { get; set; } = DateTime.Now;

    [Column("udlaantAfleveretDato")]
    public DateTime? UdlaantAfleveretDato { get; set; }

    // Navigation properties til EF Core
    [ForeignKey("InventarId")]
    public virtual Inventar? Inventar { get; set; }

    [ForeignKey("EleverId")]
    public virtual Elev? Elev { get; set; }
}