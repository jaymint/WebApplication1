using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public LoginModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Username and Password are required.";
            return Page();
        }

        // Hash the input password
        string hashedPassword;
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
            hashedPassword = Convert.ToBase64String(hashedBytes);
        }

        // Authenticate user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == Username && u.Password == hashedPassword);

        if (user == null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        // Set session or cookie for authentication
        HttpContext.Session.SetString("Username", Username);

        return RedirectToPage("/Index");
    }
}

