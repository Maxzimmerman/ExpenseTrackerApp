using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Migrations;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class UserManage : BaseController
    {
        private readonly IUserManageService _userManageService;
        private readonly IUserRepository _userRepository;

        public UserManage(
            IUserManageService userManageService,
            IFooterRepository footerRepository,
            IUserRepository userRepository) : base(footerRepository)
        {
            _userManageService = userManageService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            SignUpviewModel signUpviewModel = new SignUpviewModel();
            return View(signUpviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpviewModel signUpviewModel)
        {
            var existingUser = _userRepository.findByEmail(signUpviewModel.Email);

            if (existingUser != null)
            {
                return RedirectToAction("UserAlreadyExists", "UserManage");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManageService.SignUp(signUpviewModel);
                return View("VerifyEmail");
            }
            return View("BadRequest");
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            SignInviewModel signInviewModel = new SignInviewModel();
            return View(signInviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInviewModel signInviewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.findByEmail(signInviewModel.Email);
                if (user == null)
                {
                    return RedirectToAction("AccessDenied", "UserManage");
                }

                if (!user.EmailConfirmed)
                {
                    return RedirectToAction("VerifyEmail", "UserManage");
                }

                if (await _userManageService.SignIn(signInviewModel))
                {
                    return RedirectToAction("Home", "Home");
                }
                return RedirectToAction("AccessDenied", "UserManage");
            }
            return RedirectToAction("AccessDenied", "UserManage");
        }

        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _userManageService.SignOut();
            return RedirectToAction("Home", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SettingsProfile()
        {
            try
            {
                var user = await _userManageService.GetCurrentUser(User);
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SettingsProfile(ApplicationUser user, string currentPassword)
        {
            try
            {
                var userResult = await _userManageService.UpdateUser(user, currentPassword);
                return View(userResult);
            }
            catch
            {
                return View("BadRequest");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userManageService.GetCurrentUser(User);
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (await _userManageService.ConfirmEmail(userId, token))
                return RedirectToAction("SignIn", "UserManage");
            return View("Error");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserAlreadyExists()
        {
            return View();
        }
    }
}