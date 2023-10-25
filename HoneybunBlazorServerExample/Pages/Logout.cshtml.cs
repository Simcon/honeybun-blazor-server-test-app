using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoneybunBlazorServerExample.Pages
{
    public class LogoutModel : PageModel
    {
        // *** START INTEGRATING HONEYBUN AUTH
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        // *** END INTEGRATING HONEYBUN AUTH
    }
}
