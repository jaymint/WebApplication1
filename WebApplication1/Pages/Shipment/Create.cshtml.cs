using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

public class CreateShipmentModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateShipmentModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Shipment Shipment { get; set; }

    public List<string> Senders { get; set; } 
    public List<string> Receivers { get; set; } 
    public List<string> Cities { get; set; } 
    public List<string> BookingOffices { get; set; } 

    // Property to hold the current date and time
    public string CurrentDateTime { get; private set; }

    public string CreatedBy { get; set; }
    public string PaymentStatus { get; set; }
    public async Task OnGetAsync()
    {
        // Set the current date and time in the required format
        CurrentDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

        // Populate combo box data (replace with database queries if needed)
        Senders = await _context.Senders
        .Select(de => de.Name) // Assuming 'Name' is the field in DataEntry
        .ToListAsync();
        Receivers = await _context.dataentries
        .Select(de => de.Name) // Assuming 'Name' is the field in DataEntry
        .ToListAsync();
        Cities = await _context.Cities
           .Select(c => c.Name) // Fetch city names
           .ToListAsync();
        BookingOffices = new List<string> { "Office 1", "Office 2", "Office 3" };
    }

    public async Task<IActionResult> OnPostAsync()
    {

        // Retrieve the username from the session
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToPage("/Login"); // Redirect to login if the user is not logged in
        }

        // Set the CreatedBy property
        Shipment.CreatedBy = username;
        Shipment.ShipmentDateTime = DateTime.Now;

        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            return Page();
        }


        // Save the shipment to the database
        _context.Shipments.Add(Shipment);
        await _context.SaveChangesAsync();

        // Set a confirmation message in TempData
        TempData["SuccessMessage"] = $"Shipment with ID {Shipment.Id} has been successfully created.";

        // Fetch the inserted shipment to display it
        Shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.Id == Shipment.Id);

        // Reload dropdown data
        Senders = await _context.Senders.Select(de => de.Name).ToListAsync();
        Receivers = await _context.dataentries.Select(de => de.Name).ToListAsync();
        Cities = await _context.Cities.Select(c => c.Name).ToListAsync();
        BookingOffices = new List<string> { "Office 1", "Office 2", "Office 3" };


      

        return Page(); // Stay on the same page
        //return RedirectToPage("/Index");
    }

}
