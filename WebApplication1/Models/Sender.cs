using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class Sender
{
    [Key] // Mark as primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-generate value
    [ValidateNever]
    public int Id { get; set; }
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }
    public string PhoneNumber { get; set; }

    public string? GSTIN { get; set; } // New field for GSTIN
}
