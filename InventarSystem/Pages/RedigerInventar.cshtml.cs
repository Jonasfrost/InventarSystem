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

    // Metode til at slette den enkelte inventarvare direkte (sletter også historiske/aktive udlån først)
    public async Task<IActionResult> OnPostDeleteInventarAsync(int id)
    {
        var vare = await _context.Inventar.FindAsync(id);

        if (vare == null)
        {
            return RedirectToPage("/Tilgængelig");
        }

        try
        {
            // 1. Slet alle udlånstilknytninger for varen først
            var tilhoerendeUdlaan = await _context.Udlaant
                .Where(u => u.InventarId == id)
                .ToListAsync();

            if (tilhoerendeUdlaan.Any())
            {
                _context.Udlaant.RemoveRange(tilhoerendeUdlaan);
            }

            // 2. Slet selve varen fra lageret
            _context.Inventar.Remove(vare);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Tilgængelig");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Kunne ikke slette varen: {ex.Message}");
            return Page();
        }
    }
}