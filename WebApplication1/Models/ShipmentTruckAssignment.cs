using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ShipmentTruckAssignment
{
    public int Id { get; set; }

    [Required]
    public int ShipmentId { get; set; }

    [Required]
    public int TruckId { get; set; }

    [Required]
    public DateTime AssignmentDate { get; set; } // New property for the assignment date

    // Navigation properties
    [ForeignKey("ShipmentId")]
    [ValidateNever]
    public Shipment Shipment { get; set; }

    [ForeignKey("TruckId")]
    [ValidateNever]
    public Truck Truck { get; set; }
}

