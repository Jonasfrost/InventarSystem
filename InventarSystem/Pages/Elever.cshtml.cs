using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class EleverModel : PageModel
{
    private readonly InventarDbContext _context;

    public EleverModel(InventarDbContext context)
    {
        _context = context;
    }

    public List<Elev> EleverListe { get; set; } = new();

    public SelectList InstruktorDropdown { get; set; } = default!;

    public string CurrentFilter { get; set; } = string.Empty;

    [BindProperty]
    public Elev NyElev { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string searchString)
    {
        CurrentFilter = searchString;
        await HentDataAsync(searchString);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await HentDataAsync(CurrentFilter);
            return Page();
        }

        _context.Elev.Add(NyElev);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task HentDataAsync(string searchString)
    {
        // 1. Hent alle elever først, så EF Core ValueConverter dekrypterer Fn, Ln, Mail og Mobil i memory
        var alleElever = await _context.Elev.ToListAsync();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int idSearch))
            {
                // Søg på ElevId eller i de dekrypterede for- og efternavne
                EleverListe = alleElever
                    .Where(e => e.ElevId == idSearch
                             || (!string.IsNullOrEmpty(e.Fn) && e.Fn.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                             || (!string.IsNullOrEmpty(e.Ln) && e.Ln.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            else
            {
                // Søg udelukkende i de dekrypterede navne (case-insensitive)
                EleverListe = alleElever
                    .Where(e => (!string.IsNullOrEmpty(e.Fn) && e.Fn.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                             || (!string.IsNullOrEmpty(e.Ln) && e.Ln.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
        }
        else
        {
            EleverListe = alleElever;
        }

        // 2. Hent instruktører til oprettelses-dropdownen
        var instruktoerer = await _context.Instruktor.ToListAsync();
        InstruktorDropdown = new SelectList(
            instruktoerer.Select(i => new {
                Id = i.InstruktorId,
                Navn = $"{i.Fn} {i.Ln}"
            }),
            "Id",
            "Navn"
        );
    }
}