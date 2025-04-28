using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SearchShipmentsByDateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SearchShipmentsByDateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? SearchDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string BookingOffice { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool UnassignedOnly { get; set; } // Flag to search unassigned shipments

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1; // Current page number

    public int PageSize { get; set; } = 10; // Number of items per page
    public int TotalPages { get; set; } // Total number of pages

    public List<ShipmentWithTruck> Shipments { get; set; }
    public List<string> BookingOffices { get; set; } // List of unique booking offices

    public string PaymentStatus { get; set; } // "Paid" or "ToBePaid"

    public async Task OnGetAsync()
    {
        // Fetch unique booking offices for the dropdown
        BookingOffices = await _context.Shipments
            .Select(s => s.BookingOffice)
            .Distinct()
            .ToListAsync();

        // Query to fetch shipments with sender and receiver names
        var query = from shipment in _context.Shipments
                    join sender in _context.Senders on shipment.SenderId equals sender.Id
                    join receiver in _context.dataentries on shipment.ReceiverId equals receiver.Id
                    join assignment in _context.ShipmentTruckAssignments
                        on shipment.Id equals assignment.ShipmentId into shipmentAssignments
                    from assignment in shipmentAssignments.DefaultIfEmpty()
                    join truck in _context.Trucks
                        on assignment.TruckId equals truck.Id into truckAssignments
                    from truck in truckAssignments.DefaultIfEmpty()
                    select new ShipmentWithTruck
                    {
                        Id = shipment.Id,
                        SenderName = sender.Name, // Fetch SenderName
                        ReceiverName = receiver.Name, // Fetch ReceiverName
                        City = shipment.City,
                        BookingOffice = shipment.BookingOffice,
                        ShipmentDateTime = shipment.ShipmentDateTime,
                        Description = shipment.Description,
                        NumberOfItems = shipment.NumberOfItems,
                        TotalWeight = shipment.TotalWeight,
                        Price = shipment.Price,
                        PaymentStatus = shipment.PaymentStatus,
                        TruckNumber = truck != null ? truck.TruckNumber : "Unassigned"
                    };

        if (UnassignedOnly)
        {
            query = query.Where(s => s.TruckNumber == "Unassigned");
        }
        else
        {
            // Filter by date if provided
            if (SearchDate.HasValue)
            {
                query = query.Where(s => s.ShipmentDateTime.Date == SearchDate.Value.Date);
            }

            // Filter by booking office if provided
            if (!string.IsNullOrEmpty(BookingOffice))
            {
                query = query.Where(s => s.BookingOffice == BookingOffice);
            }
        }

        // Calculate total pages
        int totalItems = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        // Apply pagination
        Shipments = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}

public class ShipmentWithTruck
{
    public int Id { get; set; }
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string City { get; set; }
    public string BookingOffice { get; set; }
    public DateTime ShipmentDateTime { get; set; }
    public string Description { get; set; }
    public int NumberOfItems { get; set; }
    public double TotalWeight { get; set; }
    public decimal Price { get; set; }
    public string TruckNumber { get; set; } // Truck number or "Unassigned"

    public string PaymentStatus { get; set; } // "Paid" or "ToBePaid"

}
