using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class OpretUdlaanModel : PageModel
{
    private readonly InventarDbContext _context;

    public OpretUdlaanModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Udlaant Udlaan { get; set; } = new();

    // Bruges til at vise listen over elever i dropdown/søgefeltet
    public List<Elev> Elever { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int inventarId)
    {
        Elever = await _context.Elever.ToListAsync();

        Udlaan = new Udlaant
        {
            inventarId = inventarId,
            udlaantAntal = 1,
            udlaantDato = DateTime.Now
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Fail-safe 1: Tjek om den valgte elev findes i databasen
        var elevEksisterer = await _context.Elever.AnyAsync(e => e.ElevId == Udlaan.eleverId);
        if (!elevEksisterer)
        {
            ModelState.AddModelError("Udlaan.eleverId", "Vælg venligst en gyldig elev fra listen.");
        }

        // Fail-safe 2: Tjek om varen findes og om der er nok på lager
        var vare = await _context.Inventar.FindAsync(Udlaan.inventarId);
        if (vare == null)
        {
            ModelState.AddModelError(string.Empty, "Varen blev ikke fundet på lageret.");
        }
        else if (Udlaan.udlaantAntal <= 0)
        {
            ModelState.AddModelError("Udlaan.udlaantAntal", "Antal skal være mindst 1.");
        }
        else if (vare.Antal < Udlaan.udlaantAntal)
        {
            ModelState.AddModelError("Udlaan.udlaantAntal", $"Der er kun {vare.Antal} stk. på lager.");
        }

        if (!ModelState.IsValid)
        {
            // Genindlæs eleverne hvis siden skal vises igen pga. fejl
            Elever = await _context.Elever.ToListAsync();
            return Page();
        }

        // Træk antal fra lageret og opret udlån
        vare!.Antal -= Udlaan.udlaantAntal;
        Udlaan.udlaantDato = DateTime.Now;
        _context.Udlaant.Add(Udlaan);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}