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

    public List<Sender> Senders { get; set; } 
    public List<DataEntry> Receivers { get; set; } 
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
        Senders = await _context.Senders.ToListAsync();
        Receivers = await _context.dataentries.ToListAsync();
        Cities = await _context.Cities
           .Select(c => c.Name) // Fetch city names
           .ToListAsync();
        BookingOffices = new List<string> { "Sarkhej", "Aslali", "Kalupur" };
        // If a shipment exists, populate SenderName and ReceiverName
        if (Shipment != null && Shipment.Id > 0)
        {
            Shipment = await _context.Shipments
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .Where(s => s.Id == Shipment.Id)
                .Select(s => new Shipment
                {
                    Id = s.Id,
                    SenderId = s.SenderId,
                    ReceiverId = s.ReceiverId,
                    SenderName = s.Sender.Name, // Retrieve SenderName
                    ReceiverName = s.Receiver.Name, // Retrieve ReceiverName
                    City = s.City,
                    BookingOffice = s.BookingOffice,
                    ShipmentDateTime = s.ShipmentDateTime,
                    Description = s.Description,
                    NumberOfItems = s.NumberOfItems,
                    TotalWeight = s.TotalWeight,
                    Price = s.Price,
                    PaymentStatus = s.PaymentStatus,
                    CreatedBy = s.CreatedBy
                })
                .FirstOrDefaultAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {

        Shipment.ShipmentDateTime = DateTime.Now;
     
        if (!ModelState.IsValid)
        {
         
            return Page();
        }


        // Save the shipment to the database
        _context.Shipments.Add(Shipment);
        await _context.SaveChangesAsync();

        // Set a confirmation message in TempData
        TempData["SuccessMessage"] = $"Shipment with ID {Shipment.Id} has been successfully created.";

        // Fetch the inserted shipment to display it
        Shipment = await _context.Shipments
            .Include(s => s.Sender)
            .Include(s => s.Receiver)
            .Where(s => s.Id == Shipment.Id)
            .Select(s => new Shipment
            {
                Id = s.Id,
                SenderId = s.SenderId,
                ReceiverId = s.ReceiverId,
                SenderName = s.Sender.Name, // Retrieve SenderName
                ReceiverName = s.Receiver.Name, // Retrieve ReceiverName
                City = s.City,
                BookingOffice = s.BookingOffice,
                ShipmentDateTime = s.ShipmentDateTime,
                Description = s.Description,
                NumberOfItems = s.NumberOfItems,
                TotalWeight = s.TotalWeight,
                Price = s.Price,
                PaymentStatus = s.PaymentStatus,
                CreatedBy = s.CreatedBy
            })
            .FirstOrDefaultAsync();

        // Reload dropdown data
        Senders = await _context.Senders.ToListAsync();
        Receivers = await _context.dataentries.ToListAsync();
        Cities = await _context.Cities.Select(c => c.Name).ToListAsync();
        BookingOffices = new List<string> { "Sarkhej", "Aslali", "Kalupur" };


      

        return Page(); // Stay on the same page
        //return RedirectToPage("/Index");
    }

}
