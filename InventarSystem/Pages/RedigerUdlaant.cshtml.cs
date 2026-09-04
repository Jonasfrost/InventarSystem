using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class RedigerUdlaantModel : PageModel
{
    private readonly InventarDbContext _context;

    public RedigerUdlaantModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Udlaant Udlaan { get; set; } = default!;

    // Gemmer det aktuelle lagerantal til visning i UI
    public int LagerAntal { get; set; }

    // Beregnet maks tilladt antal udlån for denne post
    public int MaxMulige { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var udlaan = await _context.Udlaant
            .Include(u => u.Elev)
            .FirstOrDefaultAsync(m => m.UdlaantId == id);

        if (udlaan == null)
        {
            return NotFound();
        }

        Udlaan = udlaan;

        // Hent den tilhørende vare for at kende lagerstatus
        var vare = await _context.Inventar.FindAsync(Udlaan.InventarId);
        if (vare != null)
        {
            LagerAntal = vare.Antal;
            MaxMulige = Udlaan.UdlaantAntal + vare.Antal;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Hent det oprindelige udlån fra databasen
        var eksisterendeUdlaan = await _context.Udlaant
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UdlaantId == Udlaan.UdlaantId);

        if (eksisterendeUdlaan == null)
        {
            return NotFound();
        }

        // 2. Hent den tilhørende inventarvare
        var inventarVare = await _context.Inventar.FindAsync(Udlaan.InventarId);

        if (inventarVare != null)
        {
            LagerAntal = inventarVare.Antal;
            MaxMulige = eksisterendeUdlaan.UdlaantAntal + inventarVare.Antal;

            // 3. Beregn forskellen
            int forskel = Udlaan.UdlaantAntal - eksisterendeUdlaan.UdlaantAntal;

            // 4. Validering: Tjek om der er nok på lager
            if (forskel > 0 && inventarVare.Antal < forskel)
            {
                ModelState.AddModelError("Udlaan.UdlaantAntal", $"Der er kun {inventarVare.Antal} stk. tilbage på lager. Du kan højst øge udlånet til {MaxMulige} stk.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 5. Opdater lageret med forskellen
            inventarVare.Antal -= forskel;
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // 6. Gem opdateringen
        _context.Attach(Udlaan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Udlaant.Any(e => e.UdlaantId == Udlaan.UdlaantId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostDeleteSingleAsync(int id)
    {
        var udlaan = await _context.Udlaant.FindAsync(id);

        if (udlaan != null)
        {
            var inventarVare = await _context.Inventar.FindAsync(udlaan.InventarId);
            if (inventarVare != null)
            {
                inventarVare.Antal += udlaan.UdlaantAntal;
            }

            _context.Udlaant.Remove(udlaan);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Index");
    }
}