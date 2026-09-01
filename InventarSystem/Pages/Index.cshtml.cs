using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class IndexModel : PageModel
{
    private readonly InventarDbContext _context;

    public IndexModel(InventarDbContext context)
    {
        _context = context;
    }

    public List<Udlaant> UdlaanteVarer { get; set; } = new();

    public string CurrentFilter { get; set; } = string.Empty;

    public async Task OnGetAsync(string searchString)
    {
        CurrentFilter = searchString;

        var query = _context.Udlaant
            .Include(u => u.Inventar)
            .AsQueryable();

        // Filtrering ud fra søgning
        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int idSearch))
            {
                query = query.Where(u => u.udlaantId == idSearch
                                      || u.eleverId == idSearch
                                      || (u.Inventar != null && u.Inventar.Item.Contains(searchString)));
            }
            else
            {
                query = query.Where(u => u.Inventar != null && u.Inventar.Item.Contains(searchString));
            }
        }

        // Hent data og sorter: Overskredne datoer først, herefter efter nærmeste afleveringsdato
        var liste = await query.ToListAsync();

        UdlaanteVarer = liste
            .OrderByDescending(u => u.udlaantAfleveretDato.HasValue && u.udlaantAfleveretDato.Value < DateTime.Now) // Overskredne placeres øverst (true kommer før false)
            .ThenBy(u => u.udlaantAfleveretDato) // Derefter sorteret efter hvornår de skal afleveres
            .ToList();
    }
}