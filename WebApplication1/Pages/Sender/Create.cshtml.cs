using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateSenderModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateSenderModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Sender Sender { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Senders.Add(Sender);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}
