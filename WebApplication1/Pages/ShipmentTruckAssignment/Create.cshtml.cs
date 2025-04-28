using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class CreateShipmentTruckAssignmentModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateShipmentTruckAssignmentModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public ShipmentTruckAssignment ShipmentTruckAssignment { get; set; }

    public List<Shipment> Shipments { get; set; }
    public List<Truck> Trucks { get; set; }

    public async Task OnGetAsync()
    {
        // Fetch shipments that are not assigned to any truck
        var assignedShipmentIds = await _context.ShipmentTruckAssignments
            .Select(sta => sta.ShipmentId)
            .ToListAsync();

        Shipments = await _context.Shipments
         .Where(s => !assignedShipmentIds.Contains(s.Id))
         .Include(s => s.Sender) // Include Sender navigation property
         .Include(s => s.Receiver) // Include Receiver navigation property
         .Select(s => new Shipment
         {
             Id = s.Id,
             SenderName = s.Sender.Name, // Fetch SenderName
             ReceiverName = s.Receiver.Name, // Fetch ReceiverName
             City = s.City,
             BookingOffice = s.BookingOffice,
             ShipmentDateTime = s.ShipmentDateTime,
             Description = s.Description,
             NumberOfItems = s.NumberOfItems,
             TotalWeight = s.TotalWeight,
             Price = s.Price
         })
         .ToListAsync();

        // Fetch all trucks for the dropdown
        Trucks = await _context.Trucks.ToListAsync();

        // Ensure Shipments and Trucks are not null
        if (Shipments == null)
        {
            Shipments = new List<Shipment>();
        }

        if (Trucks == null)
        {
            Trucks = new List<Truck>();
        }

        Console.WriteLine($"Fetched {Shipments?.Count ?? 0} unassigned shipments and {Trucks?.Count ?? 0} trucks.");
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload dropdown data in case of validation errors
            Shipments = await _context.Shipments.ToListAsync();
            Trucks = await _context.Trucks.ToListAsync();
            return Page();
        }

        // Set the assignment date if not already set
        if (ShipmentTruckAssignment.AssignmentDate == default)
        {
            ShipmentTruckAssignment.AssignmentDate = DateTime.Now;
        }

        // Save the assignment to the database
        _context.ShipmentTruckAssignments.Add(ShipmentTruckAssignment);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }

}
