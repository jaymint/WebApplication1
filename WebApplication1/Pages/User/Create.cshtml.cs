using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

public class CreateUserModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateUserModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public User User { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Hash the password before saving it to the database
        using (var sha256 = SHA256.Create())
        {
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(User.Password));
            User.Password = Convert.ToBase64String(hashedPassword);
        }

        _context.users.Add(User);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Login");
    }
}
