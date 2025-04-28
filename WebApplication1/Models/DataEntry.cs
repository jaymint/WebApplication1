/*public class DataEntry 
{
    public DataEntry()
    {
        Id = 0;
        Name = string.Empty;
        Email = string.Empty;
    }
    public DataEntry(int id, string name, string email):this()
    {
        Id = id;
        Name = name;
        Email = email;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class DataEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [ValidateNever]
    public int Id { get; set; }
    public string Name { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }
    public string MobileNumber { get; set; } // New field
    public string CityName { get; set; }    // New field
}

