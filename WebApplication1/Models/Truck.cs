using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class Truck
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [ValidateNever]
    public int Id { get; set; }
    public string TruckNumber { get; set; } // Truck registration number
    public string Model { get; set; } // Truck model
    public int Capacity { get; set; } // Capacity in tons
    public string DriverName { get; set; } // Driver's name
    public string OwnerName { get; set; } // Owner's name
    public string OwnerMobileNumber { get; set; } // Owner's mobile number
    public string DriverMobileNumber { get; set; } // Driver's mobile number
}
