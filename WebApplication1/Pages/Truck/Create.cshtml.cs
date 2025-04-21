using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateTruckModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateTruckModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Truck Truck { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Trucks.Add(Truck);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}
