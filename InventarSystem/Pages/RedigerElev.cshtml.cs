using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class RedigerElevModel : PageModel
{
    private readonly InventarDbContext _context;

    public RedigerElevModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Elev Elev { get; set; } = default!;

    public List<Instruktor> Instruktoerer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Instruktoerer = await _context.Instruktor.ToListAsync();
        var elev = await _context.Elever.FirstOrDefaultAsync(m => m.ElevId == id);
        if (elev == null)
        {
            return NotFound();
        }

        Elev = elev;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Elev.InstruktorId.HasValue)
        {
            var instruktorEksisterer = await _context.Instruktor
                .AnyAsync(i => i.InstruktorId == Elev.InstruktorId.Value);

            if (!instruktorEksisterer)
            {
                ModelState.AddModelError("Elev.InstruktorId", $"Instruktør med ID {Elev.InstruktorId} findes ikke.");
                return Page();
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Elev).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Elever.Any(e => e.ElevId == Elev.ElevId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Elever");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var elevTilSletning = await _context.Elever.FindAsync(id);

        if (elevTilSletning != null)
        {
            _context.Elever.Remove(elevTilSletning);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Elever");
    }
}