using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class CreateSenderModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateSenderModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Sender Sender { get; set; }
    public List<Sender> Senders { get; set; } = new List<Sender>();


    public async Task OnGetAsync()
    {
        // Fetch all senders from the database
       
       
        Senders = await _context.Senders
            .Select(d => new Sender
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                GSTIN = d.GSTIN

            })
            .ToListAsync();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
           // return Page();
        }

        if (Sender.Id > 0)
        {
            // Update existing sender
            var existingSender = await _context.Senders.FindAsync(Sender.Id);
            if (existingSender != null)
            {
                existingSender.Name = Sender.Name;
                existingSender.Email = Sender.Email;
                existingSender.PhoneNumber = Sender.PhoneNumber;
                // Update other properties as needed
                existingSender.GSTIN = Sender.GSTIN; // Update GSTIN field  

                _context.Senders.Update(existingSender);
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            // Create new sender
            _context.Senders.Add(Sender);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Sender/Create");
    }

}
