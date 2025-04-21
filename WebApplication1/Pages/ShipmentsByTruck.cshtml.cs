using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

    public async Task OnGetAsync()
    {
        // Fetch all trucks for the dropdown
        Trucks = await _context.Trucks.ToListAsync();

        if (TruckId.HasValue)
        {
            // Fetch shipments for the selected truck
            Shipments = await _context.ShipmentTruckAssignments
                .Where(sta => sta.TruckId == TruckId.Value)
                .Include(sta => sta.Shipment) // Include the Shipment navigation property
                .Select(sta => sta.Shipment)
                .ToListAsync();

            // Get the selected truck number
            var selectedTruck = await _context.Trucks.FindAsync(TruckId.Value);
            SelectedTruckNumber = selectedTruck?.TruckNumber;
        }
    }
}
