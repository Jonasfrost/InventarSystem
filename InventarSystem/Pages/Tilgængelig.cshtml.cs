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

    public async Task OnGetAsync()
    {
        PaaLager = await _context.Inventar.ToListAsync();
    }

    public async Task<IActionResult> OnPostUdlaanAsync(int id)
    {
        var vare = await _context.Inventar.FindAsync(id);
        var elev = await _context.Elever.FirstOrDefaultAsync();

        if (elev == null)
        {
            ModelState.AddModelError(string.Empty, "Der skal oprettes mindst én elev i databasen før du kan udlåne.");
            PaaLager = await _context.Inventar.ToListAsync();
            return Page();
        }

        if (vare != null && vare.Antal > 0)
        {
            vare.Antal--;

            _context.Udlaant.Add(new Udlaant
            {
                eleverId = elev.ElevId,
                inventarId = vare.InventarId,
                udlaantAntal = 1,
                udlaantDato = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Tilgængelig");
    }
}