using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    public class Settings : Controller
    {
        [Authorize]
        public IActionResult General()
        {
            return RedirectToAction("SettingsProfile", "UserManage");
        }
    }
}
