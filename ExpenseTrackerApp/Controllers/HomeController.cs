using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Home()
        {
            if(User.Identity.IsAuthenticated)
            {
                var currentUser = (ClaimsIdentity)User.Identity;
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                var user = _applicationDbContext.applicationUsers.FirstOrDefault(u =>  u.Id == currentUserId);
                return View(user);
            }
            else
            {
                return RedirectToAction("SignIn", "UserManage");
            }
        }

        public IActionResult Budgets()
        {
            return View();
        }

        public IActionResult Goals()
        {
            return View();
        }

        public IActionResult Analytics()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
