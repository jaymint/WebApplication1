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

        // Pagination properties
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalSenders { get; set; }

    public async Task OnGetAsync()
    {
        // Fetch all senders from the database


        IQueryable<Sender> query = _context.Senders
            .Select(d => new Sender
            {
                Id = d.Id,
                Name = d.Name,
                Address = d.Address,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                GSTIN = d.GSTIN,
                IsGstinVerified = d.IsGstinVerified

            });
            TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)PageSize);

        Senders = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
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
                existingSender.Address = Sender.Address; // Update Address field    
                existingSender.PhoneNumber = Sender.PhoneNumber;
                // Update other properties as needed
                existingSender.GSTIN = Sender.GSTIN; // Update GSTIN field  
                existingSender.IsGstinVerified = Sender.IsGstinVerified; // Update IsGstinVerified field    

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
        TempData["SuccessMessage"] = "Sender details have been successfully saved.";
        return RedirectToPage("/Sender/Create");
    }

}
