namespace InventarSystem.Models;

public static class InventarData
{
    public static List<Inventar> InventarListe { get; set; } = new List<Inventar>
    {
        new Inventar { InventarId = 1, Item = "Dell 27 Monitor", Sn = "SN-98213", Antal = 5 },
        new Inventar { InventarId = 2, Item = "Logitech MX Keys", Sn = "SN-11092", Antal = 10 }
    };

    public static List<Udlaant> UdlaantListe { get; set; } = new List<Udlaant>
    {
        new Udlaant { UdlaantId = 1, EleverId = 1, InventarId = 2, UdlaantAntal = 1, UdlaantDato = DateTime.Now.AddDays(-2) }
    };
}