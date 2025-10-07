using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class Expense
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } // Description of the expense

    [Required]
    public decimal Amount { get; set; } // Expense amount

    [Required]
    public DateTime Date { get; set; } // Date of the expense

    public string CreatedBy { get; set; } // User who created the expense
}
