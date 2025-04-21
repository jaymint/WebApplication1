using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DataEntry DataEntry { get; set; }

    public List<string> Cities { get; set; } = new List<string>();

    public async Task OnGetAsync()
    {
        // Fetch the list of cities from the Cities table
        Cities = await _context.Cities
            .Select(c => c.Name) // Assuming 'Name' is the field in the Cities table
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.dataentries.Add(DataEntry);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}


