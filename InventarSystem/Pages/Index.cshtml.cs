using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventarSystem.Data;
using InventarSystem.Models;

namespace InventarSystem.Pages;

public class IndexModel : PageModel
{
    private readonly InventarDbContext _context;

    public IndexModel(InventarDbContext context)
    {
        _context = context;
    }

    public List<Udlaant> UdlaanteVarer { get; set; } = new();

    public async Task OnGetAsync()
    {
        UdlaanteVarer = await _context.Udlaant
            .Include(u => u.Inventar) 
            .ToListAsync();
    }
}