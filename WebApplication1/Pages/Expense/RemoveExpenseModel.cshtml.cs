using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class RemoveExpenseModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public RemoveExpenseModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Expense Expense { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Fetch the expense by ID
        Expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

        if (Expense == null)
        {
            TempData["ErrorMessage"] = "Expense not found.";
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Fetch the expense by ID
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            TempData["ErrorMessage"] = "Expense not found.";
            return RedirectToPage("/Index");
        }

        // Remove the expense
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Expense removed successfully.";
        return RedirectToPage("/Index");
    }
}
