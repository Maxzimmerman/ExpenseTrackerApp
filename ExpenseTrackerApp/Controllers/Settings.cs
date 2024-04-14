using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    public class Settings : Controller
    {
        public IActionResult General()
        {
            return View();
        }
    }
}
