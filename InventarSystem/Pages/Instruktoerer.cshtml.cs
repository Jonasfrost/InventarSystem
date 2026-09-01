using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class InstruktoererModel : PageModel
{
    private readonly InventarDbContext _context;

    public InstruktoererModel(InventarDbContext context)
    {
        _context = context;
    }

    public List<Instruktor> InstruktoerListe { get; set; } = new();

    [BindProperty]
    public Instruktor NyInstruktor { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        // Henter alle fra DB (EF Core dekrypterer automatisk med ValueConverter)
        InstruktoerListe = await _context.Instruktor.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Fjern validering på InstruktorId, da SQL Server genererer ID'et automatisk (IDENTITY)
        ModelState.Remove("NyInstruktor.InstruktorId");
        ModelState.Remove("NyInstruktor.Elever");

        if (!ModelState.IsValid)
        {
            InstruktoerListe = await _context.Instruktor.ToListAsync();
            return Page();
        }

        // Tving ID til 0, så EF Core ved det er en NY post (udløser INSERT + ValueConverter kryptering)
        NyInstruktor.InstruktorId = 0;

        _context.Instruktor.Add(NyInstruktor);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}