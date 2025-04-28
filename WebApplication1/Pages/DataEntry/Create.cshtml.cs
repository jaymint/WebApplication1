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
    public List<DataEntry> Receivers { get; set; } = new List<DataEntry>();
    public async Task OnGetAsync()
    {
        // Fetch the list of cities from the Cities table
        Cities = await _context.Cities
            .Select(c => c.Name) // Assuming 'Name' is the field in the Cities table
            .ToListAsync();

        // Fetch all receivers from the dataentries table
        Receivers = await _context.dataentries
            .Select(d => new DataEntry
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                MobileNumber = d.MobileNumber,
                CityName = d.CityName
            })
            .ToListAsync();
    }



    public async Task<IActionResult> OnPostAsync()
    {

        // Create new receiver
        if (!ModelState.IsValid)
        {
            //return Page();
        }
        if (DataEntry.Id > 0)
        {
            // Update existing receiver
            var existingReceiver = await _context.dataentries.FindAsync(DataEntry.Id);
            if (existingReceiver != null)
            {
                existingReceiver.Name = DataEntry.Name;
                existingReceiver.Email = DataEntry.Email;
                existingReceiver.MobileNumber = DataEntry.MobileNumber;
                existingReceiver.CityName = DataEntry.CityName;

                _context.dataentries.Update(existingReceiver);
                await _context.SaveChangesAsync();
            }
        }
        else
        {

            _context.dataentries.Add(DataEntry);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/DataEntry/Create");
    }






}

