using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class OpretElevModel : PageModel
{
    private readonly InventarDbContext _context;

    public OpretElevModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Elev NyElev { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Elever.Add(NyElev);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Tilgængelig");
    }
}