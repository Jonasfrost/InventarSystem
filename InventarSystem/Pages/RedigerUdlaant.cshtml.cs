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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var udlaan = await _context.Udlaant.FirstOrDefaultAsync(m => m.udlaantId == id);

        if (udlaan == null)
        {
            return NotFound();
        }

        Udlaan = udlaan;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Udlaan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Udlaant.Any(e => e.udlaantId == Udlaan.udlaantId))
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
            var inventarVare = await _context.Inventar.FindAsync(udlaan.inventarId);

            if (inventarVare != null)
            {
                inventarVare.Antal += udlaan.udlaantAntal;
            }

            _context.Udlaant.Remove(udlaan);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Index"); 
    }
}