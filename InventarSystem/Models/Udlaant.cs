using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarSystem.Models;

[Table("Udlaant")]
public class Udlaant
{
    [Key]
    [Column("udlaantId")]
    public int udlaantId { get; set; }

    [Column("eleverId")]
    public int eleverId { get; set; }

    [Column("inventarId")]
    public int inventarId { get; set; }

    [Column("udlaantAntal")]
    public int udlaantAntal { get; set; }

    [Column("udlaantDato")]
    public DateTime udlaantDato { get; set; }

    [Column("udlaantAfleveretDato")]
    public DateTime? udlaantAfleveretDato { get; set; }

    [ForeignKey("inventarId")]
    public virtual Inventar? Inventar { get; set; }
}