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
    private readonly EmailService _emailService;
    private readonly SmsService _smsService;

    public SearchShipmentsByDateModel(ApplicationDbContext context, EmailService emailService, SmsService smsService)
    {
        _context = context;
        _emailService = emailService;
        _smsService = smsService;
    }
    // Usage:



    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }


    [BindProperty(SupportsGet = true)]
    public string BookingOffice { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool UnassignedOnly { get; set; } // Flag to search unassigned shipments

    [BindProperty(SupportsGet = true)]
    public string City { get; set; } // Selected city

    public List<string> Cities { get; set; } // List of available cities


    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1; // Current page number

    public int PageSize { get; set; } = 10; // Number of items per page
    public int TotalPages { get; set; } // Total number of pages

    public List<ShipmentWithTruck> Shipments { get; set; }
    public List<string> BookingOffices { get; set; } // List of unique booking offices

    public string PaymentStatus { get; set; } // "Paid" or "ToBePaid"

    [BindProperty(SupportsGet = true)]
    public string PaymentType { get; set; }

    public async Task OnGetAsync()
    {
        // Fetch unique booking offices for the dropdown
        BookingOffices = await _context.Shipments
            .Select(s => s.BookingOffice)
            .Distinct()
            .ToListAsync();

        // Fetch distinct cities for the dropdown
        Cities = await _context.Shipments
            .Select(s => s.City)
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
            // Filter by to City if provided
            if (!string.IsNullOrEmpty(City))
            {
                query = query.Where(s => s.City == City);
            }
            if (FromDate.HasValue && ToDate.HasValue)
            {
                // Filter between the two dates (inclusive)
                query = query.Where(s => s.ShipmentDateTime.Date >= FromDate.Value.Date && s.ShipmentDateTime.Date <= ToDate.Value.Date);
            }
            else if (FromDate.HasValue)
            {
                query = query.Where(s => s.ShipmentDateTime.Date >= FromDate.Value.Date);
            }
            else if (ToDate.HasValue)
            {
                query = query.Where(s => s.ShipmentDateTime.Date <= ToDate.Value.Date);
            }

            // Filter by booking office if provided
            if (!string.IsNullOrEmpty(BookingOffice))
            {
                query = query.Where(s => s.BookingOffice == BookingOffice);
            }
        }

      
        if (!string.IsNullOrEmpty(PaymentType))
        {
            query = query.Where(s => s.PaymentStatus == PaymentType);
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

    public async Task<IActionResult> OnPostNotifyAsync(int id)
    {
        var shipment = await _context.Shipments
            .Include(s => s.Sender)
            .Include(s => s.Receiver)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shipment == null)
        {
            TempData["SuccessMessage"] = "Shipment not found.";
            return RedirectToPage();
        }

        // Only notify if ToBeBilled and not received
        if (shipment.PaymentStatus == "ToBeBilled" && !shipment.ReceivePayment)
        {
            // Send Email
            if (!string.IsNullOrEmpty(shipment.Sender?.Email))
            {
                var pdfBytes = PdfHelper.GenerateShipmentPdf(shipment);
                await _emailService.SendEmailAsync(
                    shipment.Sender.Email,
                    "Payment Reminder",
                    $"Dear {shipment.Sender.Name},\n\nYour payment of {shipment.Price} for Builty # {shipment.Id} is pending. Please make the payment at your earliest convenience. \n\n Thank you,\n Vijay Laxmi Transport",
                    pdfBytes,
                    $"Shipment_Details_{shipment.Id}.pdf"
                );
            }

            // Send SMS
            if (!string.IsNullOrEmpty(shipment.Sender?.PhoneNumber))
            {
                await SendSmsAsync(
                    shipment.Sender.PhoneNumber,
                    $"Reminder: Payment of {shipment.Price} for Builty # {shipment.Id} is pending. Please pay soon.\nVijay Laxmi Transport"
                );
            }

            TempData["SuccessMessage"] = $"Notification sent to sender for shipment ID {shipment.Id}.";
        }
        else
        {
            TempData["SuccessMessage"] = "Notification not sent. Either payment is already received or not ToBeBilled.";
        }

        return RedirectToPage();
    }

    // Dummy implementations for demonstration

    private Task SendSmsAsync(string phoneNumber, string message)
    {
        // Integrate with your SMS service here
        //_smsService.SendSmsAsync(phoneNumber, message);
        return Task.CompletedTask;
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


