using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Services.IServices;
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
        private readonly IUserManageService _userManageService;
        private readonly IMessageRepository _messageRepository;

        public HomeController(ILogger<HomeController> logger, 
            IUserRepository userRepository, 
            IUserManageService userManageService,
            IFooterRepository footerRepository,
            IMessageRepository messageRepository) : base(footerRepository)
        {
            _logger = logger;
            _userManageService = userManageService;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public IActionResult Home()
        {
            if(User.Identity.IsAuthenticated)
            {
                try
                {
                    var user = _userRepository.getUserById(_userManageService.GetCurrentUserId(User));
                    return View(user);
                }
                catch
                {
                    return RedirectToAction("SignIn", "UserManage");
                }
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
