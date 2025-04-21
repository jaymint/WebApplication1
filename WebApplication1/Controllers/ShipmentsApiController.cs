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
        var shipment = await _context.Shipments.FindAsync(id);

        if (shipment == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            shipment.SenderName,
            shipment.ReceiverName,
            shipment.City,
            shipment.BookingOffice,
            shipment.ShipmentDateTime,
            shipment.Description,
            shipment.NumberOfItems,
            shipment.TotalWeight,
            shipment.Price
        });
    }
}

