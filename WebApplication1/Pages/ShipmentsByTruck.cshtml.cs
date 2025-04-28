using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ShipmentsByTruckModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ShipmentsByTruckModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Truck> Trucks { get; set; }
    public List<Shipment> Shipments { get; set; }
    public string SelectedTruckNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? TruckId { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? AssignmentDate { get; set; } // New property for assignment date

    public async Task OnGetAsync()
    {
        // Fetch all trucks for the dropdown
        Trucks = await _context.Trucks.ToListAsync();

        if (TruckId.HasValue)
        {

            // Fetch shipments for the selected truck and assignment date
            var query = _context.ShipmentTruckAssignments
                .Where(sta => sta.TruckId == TruckId.Value)
                .Where(sta => sta.AssignmentDate.Date == DateTime.Today) // Filter by assignment date
                .Include(sta => sta.Shipment) // Include the Shipment navigation property
                .ThenInclude(s => s.Sender) // Include Sender navigation property   
                .Include(sta => sta.Shipment.Receiver); // Include Receiver navigation property  
           // Fetch shipments with sender and receiver names
            Shipments = await query
            .Select(sta => new Shipment
            {
                Id = sta.Shipment.Id,
                SenderName = sta.Shipment.Sender.Name, // Fetch SenderName
                ReceiverName = sta.Shipment.Receiver.Name, // Fetch ReceiverName
                City = sta.Shipment.City,
                BookingOffice = sta.Shipment.BookingOffice,
                ShipmentDateTime = sta.Shipment.ShipmentDateTime,
                Description = sta.Shipment.Description,
                NumberOfItems = sta.Shipment.NumberOfItems,
                TotalWeight = sta.Shipment.TotalWeight,
                Price = sta.Shipment.Price,
                PaymentStatus = sta.Shipment.PaymentStatus
            })
            .ToListAsync();

            // Get the selected truck number
            var selectedTruck = await _context.Trucks.FindAsync(TruckId.Value);
            SelectedTruckNumber = selectedTruck?.TruckNumber;
        }
    }
}
