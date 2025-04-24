using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public string Username { get; private set; }

    public IActionResult OnGet()
    {
        // Check if the user is logged in
        Username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(Username))
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync();
        return RedirectToAction("/Login"); // Redirect to index after logout
    }
}

