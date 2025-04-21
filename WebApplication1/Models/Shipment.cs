using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Shipment
{
    public int Id { get; set; }
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string City { get; set; }
    public string BookingOffice { get; set; }
    public DateTime ShipmentDateTime { get; set; } // New field for date and time
    public string Description { get; set; } // Shipment description
    public int NumberOfItems { get; set; } // Number of items
    public double TotalWeight { get; set; } // Total weight of the shipment
    public decimal Price { get; set; } // Nullable to allow empty when "To be Paid" is selected
    public string PaymentStatus { get; set; } // "Paid" or "ToBePaid"


    [ValidateNever]
    public string CreatedBy { get; set; } // Username of the user who created the shipment

}
