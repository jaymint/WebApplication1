using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class FillShipmentAmountsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public FillShipmentAmountsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? SearchShipmentId { get; set; } // For searching the shipment

    [BindProperty]
    public int ShipmentId { get; set; } // For updating the shipment

    [BindProperty]
    public decimal Amount { get; set; } // Amount to be updated

    public bool ShipmentFound { get; set; } // Indicates if the shipment was found

    // Additional shipment details
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string City { get; set; }
    public string BookingOffice { get; set; }
    public string PaymentStatus { get; set; }

    [BindProperty]
    public bool ReceivePayment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (SearchShipmentId.HasValue)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .FirstOrDefaultAsync(s => s.Id == SearchShipmentId.Value);

            if (shipment != null)
            {
                ShipmentId = shipment.Id;
                Amount = shipment.Price;
                SenderName = shipment.Sender?.Name;
                ReceiverName = shipment.Receiver?.Name;
                City = shipment.City;
                BookingOffice = shipment.BookingOffice;
                PaymentStatus = shipment.PaymentStatus;
                ShipmentFound = true;
                ReceivePayment = shipment.ReceivePayment;
            }
            else
            {
                ShipmentFound = false;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var shipment = await _context.Shipments.FindAsync(ShipmentId);
        if (shipment == null)
        {
            return NotFound();
        }

        shipment.Price = Amount; // Update the shipment amount
        shipment.ReceivePayment = ReceivePayment;
        await _context.SaveChangesAsync();

        // Set a confirmation message in TempData
        TempData["SuccessMessage"] = $"Builty with ID {ShipmentId} has been successfully updated.";

        return RedirectToPage(); // Redirect to the home page or another relevant page
    }
}

