namespace InventarSystem.Models;

public class Udstyr
{
    public int Id { get; set; }
    public string Navn { get; set; } = string.Empty;
    public string Serienummer { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}