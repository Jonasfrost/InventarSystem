using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class OpretModel : PageModel
{
    private readonly InventarDbContext _context;

    public OpretModel(InventarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Inventar NytInventar { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Inventar.Add(NytInventar);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Tilgængelig");
    }
}