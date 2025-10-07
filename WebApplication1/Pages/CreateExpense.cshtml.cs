using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class CreateExpenseModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateExpenseModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Expense Expense { get; set; }
    public List<string> Categories { get; set; } // Predefined categories

    public void OnGet()
    {
        // Define predefined categories
        Categories = new List<string>
        {
            "Travel",
            "Food",
            "Office Supplies",
            "Utilities",
            "Miscellaneous",
            "Salary",
            "Office Rent",
            "Truck Rent",
            "Fuel",
            "Toll",
            "Telephone Bill",
            "Internet Bill",
            "Stationery",
            "Repair & Maintenance",
            "Insurance",
            "Hamali"
          
        }.OrderBy(c => c).ToList(); // Sort the categories alphabetically
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload categories in case of validation errors
            Categories = new List<string>
            {
                "Travel",
                "Food",
                "Office Supplies",
                "Utilities",
                "Miscellaneous",
                "Salary",
                "Office Rent",
                "Truck Rent",
                "Fuel",
                "Toll",
                "Telephone Bill",
                "Internet Bill",
                "Stationery",
                "Repair & Maintenance",
                "Insurance",
                "Hamali"

            }.OrderBy(c => c).ToList(); // Sort the categories alphabetically
            return Page();
        }

        // Save the expense to the database
        _context.Expenses.Add(Expense);
        await _context.SaveChangesAsync();

        // Redirect to the home page or another relevant page
        return RedirectToPage("/Index");
    }
}
