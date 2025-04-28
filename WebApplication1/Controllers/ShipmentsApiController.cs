using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/shipments")]
[ApiController]
public class ShipmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ShipmentsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipment(int id)
    {
        var shipment = await _context.Shipments
            .Include(s => s.Sender) // Include the Sender navigation property
            .Include(s => s.Receiver) // Include the Receiver navigation property
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                SenderName = s.Sender.Name, // Fetch SenderName
                ReceiverName = s.Receiver.Name, // Fetch ReceiverName
                s.City,
                s.BookingOffice,
                s.ShipmentDateTime,
                s.Description,
                s.NumberOfItems,
                s.TotalWeight,
                s.Price
            })
            .FirstOrDefaultAsync();

        if (shipment == null)
        {
            return NotFound();
        }

        return Ok(shipment);
    }

}

