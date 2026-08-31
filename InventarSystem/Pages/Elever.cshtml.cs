using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public async Task OnGetAsync()
    {
        EleverListe = await _context.Elever.ToListAsync();
    }
}