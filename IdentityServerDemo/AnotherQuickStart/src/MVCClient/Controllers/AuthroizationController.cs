using Microsoft.AspNetCore.Mvc;

namespace MVCClient.Controllers
{
    public class AuthroizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
