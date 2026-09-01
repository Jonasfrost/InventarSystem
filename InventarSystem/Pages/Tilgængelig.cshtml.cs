using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class TilgaengeligModel : PageModel
{
    private readonly InventarDbContext _context;

    public TilgaengeligModel(InventarDbContext context)
    {
        _context = context;
    }

    public List<Inventar> PaaLager { get; set; } = new();

    public string CurrentFilter { get; set; } = string.Empty;

    public async Task OnGetAsync(string searchString)
    {
        CurrentFilter = searchString;

        var query = _context.Inventar.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int idSearch))
            {
                query = query.Where(i => i.InventarId == idSearch
                                      || (i.Item != null && i.Item.Contains(searchString))
                                      || (i.Sn != null && i.Sn.Contains(searchString)));
            }
            else
            {
                query = query.Where(i => (i.Item != null && i.Item.Contains(searchString))
                                      || (i.Sn != null && i.Sn.Contains(searchString)));
            }
        }

        PaaLager = await query.ToListAsync();
    }
}