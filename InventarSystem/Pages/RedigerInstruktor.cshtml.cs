using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class RedigerInstruktorModel : PageModel
{
    private readonly InventarDbContext _context;

    public RedigerInstruktorModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Instruktor Instruktor { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var instruktor = await _context.Instruktor.FirstOrDefaultAsync(m => m.InstruktorId == id);

        if (instruktor == null)
        {
            return NotFound();
        }

        Instruktor = instruktor;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Instruktor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Instruktor.Any(e => e.InstruktorId == Instruktor.InstruktorId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Instruktoerer");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var instruktor = await _context.Instruktor.FindAsync(id);

        if (instruktor != null)
        {
            var tilknyttedeElever = await _context.Elev
                .Where(e => e.InstruktorId == id)
                .ToListAsync();

            foreach (var elev in tilknyttedeElever)
            {
                elev.InstruktorId = null;
            }

            _context.Instruktor.Remove(instruktor);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Instruktoerer");
    }
}