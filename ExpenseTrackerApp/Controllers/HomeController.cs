using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, UserManager<IdentityUser> userManage)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManage;
        }

        public async Task<IActionResult> Home()
        {
            if(User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
                var currentUserId = currentUser.Id;

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
