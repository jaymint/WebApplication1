using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SearchTrucksByDateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SearchTrucksByDateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedTruckId { get; set; } // Selected truck ID

    public List<TruckAssignmentInfo> TruckAssignments { get; set; }
    public List<ShipmentDetails> Shipments { get; set; } // Shipments assigned to the selected truck

    public async Task OnGetAsync()
    {
        // Ensure StartDate and EndDate are set
        if (!StartDate.HasValue)
        {
            StartDate = DateTime.Today;
        }

        if (!EndDate.HasValue)
        {
            EndDate = DateTime.Today;
        }

        // Query to fetch unique trucks and their assignment dates
        TruckAssignments = await _context.ShipmentTruckAssignments
            .Include(sta => sta.Truck)
            .Where(sta => sta.AssignmentDate.Date >= StartDate.Value.Date && sta.AssignmentDate.Date <= EndDate.Value.Date)
            .GroupBy(sta => new { sta.Truck.Id, sta.Truck.TruckNumber, sta.Truck.DriverName }) // Group by Truck ID, TruckNumber, and DriverName
            .Select(group => new TruckAssignmentInfo
            {
                TruckId = group.Key.Id,
                TruckNumber = group.Key.TruckNumber,
                DriverName = group.Key.DriverName,
                AssignmentDate = group.Min(sta => sta.AssignmentDate) // Get the earliest assignment date for the truck
            })
            .ToListAsync();

        // If a truck is selected, fetch its assigned shipments
        if (SelectedTruckId.HasValue)
        {
            Shipments = await _context.ShipmentTruckAssignments
                .Include(sta => sta.Shipment)
                .ThenInclude(s => s.Sender)
                .Include(sta => sta.Shipment.Receiver)
                .Where(sta => sta.TruckId == SelectedTruckId.Value)
                .Select(sta => new ShipmentDetails
                {
                    ShipmentId = sta.Shipment.Id,
                    SenderName = sta.Shipment.Sender.Name,
                    ReceiverName = sta.Shipment.Receiver.Name,
                    City = sta.Shipment.City,
                    BookingOffice = sta.Shipment.BookingOffice,
                    ShipmentDate = sta.Shipment.ShipmentDateTime,
                    Description = sta.Shipment.Description,
                    NumberOfItems = sta.Shipment.NumberOfItems,
                    TotalWeight = sta.Shipment.TotalWeight,
                    Price = sta.Shipment.Price,
                    PaymentStatus = sta.Shipment.PaymentStatus
                })
                .ToListAsync();
        }
    }
}

public class TruckAssignmentInfo
{
    public int TruckId { get; set; } // Truck ID
    public string TruckNumber { get; set; }
    public string DriverName { get; set; }
    public DateTime AssignmentDate { get; set; }
}

public class ShipmentDetails
{
    public int ShipmentId { get; set; }
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string City { get; set; }
    public string BookingOffice { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string Description { get; set; }
    public int NumberOfItems { get; set; }
    public double TotalWeight { get; set; }
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; }
}
