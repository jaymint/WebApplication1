using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Shipment
{
    public int Id { get; set; }

    [Required]
    public int SenderId { get; set; } // Foreign key for Sender

    [Required]
    public int ReceiverId { get; set; } // Foreign key for Receiver

    public string City { get; set; }
    public string BookingOffice { get; set; }
    public DateTime ShipmentDateTime { get; set; }
    public string Description { get; set; }
    public int NumberOfItems { get; set; }
    public double TotalWeight { get; set; }
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; }

    public string CreatedBy { get; set; } // User who created the shipment

    // Navigation properties
    [ForeignKey("SenderId")]
    [ValidateNever]
    public Sender Sender { get; set; }

    [ForeignKey("ReceiverId")]
    [ValidateNever]
    public DataEntry Receiver { get; set; }

    // Non-mapped properties for display purposes
    [NotMapped]
    [ValidateNever]
    public string SenderName { get; set; }

    [NotMapped]
    [ValidateNever]
    public string ReceiverName { get; set; }


    public bool ReceivePayment { get; set; }

}
