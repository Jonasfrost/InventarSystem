using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class RedigerInventarModel : PageModel
{
    private readonly InventarDbContext _context;

    public RedigerInventarModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Inventar Vare { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var vare = await _context.Inventar.FirstOrDefaultAsync(m => m.InventarId == id);

        if (vare == null)
        {
            return NotFound();
        }

        Vare = vare;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Vare).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Inventar.Any(e => e.InventarId == Vare.InventarId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Tilgængelig");
    }

    public async Task<IActionResult> OnPostDeleteSelectedAsync(List<int> selectedItems)
    {
        if (selectedItems == null || !selectedItems.Any())
        {
            return RedirectToPage();
        }

        var udlaanListe = await _context.Udlaant
            .Where(u => selectedItems.Contains(u.udlaantId))
            .ToListAsync();

        foreach (var udlaan in udlaanListe)
        {
            var inventarVare = await _context.Inventar.FindAsync(udlaan.inventarId);

            if (inventarVare != null)
            {
                inventarVare.Antal += udlaan.udlaantAntal;
            }
        }

        // 5. Fjern de valgte udlån fra Udlaant-tabellen
        _context.Udlaant.RemoveRange(udlaanListe);

        // 6. Gem alle ændringer samlet i én transaktion
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}