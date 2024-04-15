using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, IUserRepository userRepository)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _userRepository = userRepository;
        }

        public IActionResult Home()
        {
            if(User.Identity.IsAuthenticated)
            {
                var currentUser = (ClaimsIdentity)User.Identity;
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                var user = _userRepository.getUserById(currentUserId);
                return View(user);
            }
            else
            {
                return RedirectToAction("SignIn", "UserManage");
            }
        }

        [Authorize]
        public IActionResult Budgets()
        {
            return View();
        }

        [Authorize]
        public IActionResult Goals()
        {
            return View();
        }

        [Authorize]
        public IActionResult Analytics()
        {
            return View();
        }

        [Authorize]
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
