using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class CreateTruckModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateTruckModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Truck Truck { get; set; }

    public List<Truck> Trucks { get; set; } = new List<Truck>();
    public async Task OnGetAsync()
    {
        // Fetch all trucks from the database
        Trucks = await _context.Trucks.ToListAsync();

        // Fetch all receivers from the dataentries table
        Trucks = await _context.Trucks
            .Select(d => new Truck
            {
                Id = d.Id,
                OwnerName = d.OwnerName,
                OwnerMobileNumber = d.OwnerMobileNumber,
                DriverName = d.DriverName,
                DriverMobileNumber = d.DriverMobileNumber,
                TruckNumber = d.TruckNumber,
                Model = d.Model,
                Capacity = d.Capacity
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            //return Page();
        }
        if (Truck.Id > 0)
        {
            // Update existing truck
            var existingTruck = await _context.Trucks.FindAsync(Truck.Id);
            if (existingTruck != null)
            {
                existingTruck.OwnerName = Truck.OwnerName;
                existingTruck.OwnerMobileNumber = Truck.OwnerMobileNumber;
                existingTruck.DriverName = Truck.DriverName;
                existingTruck.DriverMobileNumber = Truck.DriverMobileNumber;
                existingTruck.TruckNumber = Truck.TruckNumber;
                existingTruck.Model = Truck.Model;
                existingTruck.Capacity = Truck.Capacity;
                _context.Trucks.Update(existingTruck);
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            // Create new truck
            _context.Trucks.Add(Truck);
            await _context.SaveChangesAsync();
        }


        return RedirectToPage("/Truck/Create");
    }
}
