using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Migrations;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using ExpenseTrackerApp.Services;
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

        public UserManage(
            IUserManageService userManageService,
            IFooterRepository footerRepository) : base(footerRepository)
        {
            _userManageService = userManageService;
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
            if (ModelState.IsValid)
            {
                if (await _userManageService.SignUp(signUpviewModel))
                {
                    return RedirectToAction("Home", "Home");
                }
                return View("BadRequest");

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
                if (await _userManageService.SignIn(signInviewModel))
                {
                    return RedirectToAction("Home", "Home");
                }
                return RedirectToAction("AccessDenied", "UserManage");
            }
            else
            {
                return RedirectToAction("AccessDenied", "UserManage");
            }
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
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}