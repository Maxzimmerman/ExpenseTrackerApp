using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Migrations;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class UserManage : BaseController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UserManage(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IUserRepository userRepository,
            IFooterRepository footerRepository) : base(footerRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = signUpviewModel.Email, Email = signUpviewModel.Email, ApplicationUserName = signUpviewModel.Name, registeredSince = DateTime.Today };
                var result = await _userManager.CreateAsync(user, signUpviewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    return View("BadRequest");
                }
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
                var result = await _signInManager.PasswordSignInAsync(signInviewModel.Email, signInviewModel.Password, signInviewModel.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    return RedirectToAction("AccessDenied", "UserManage");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "UserManage");
            }
        }

        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Home", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SettingsProfile()
        {
            var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
            var currentUserId = currentUser.Id;

            var user = _userRepository.getUserById(currentUserId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> SettingsProfile(ApplicationUser user, string currentPassword)
        {
            var updatedUser = _userRepository.updateUser(user);

            if(!string.IsNullOrEmpty(currentPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(updatedUser);
                var changePasswordResult = await _userManager.ResetPasswordAsync(updatedUser, token, currentPassword);

                if (changePasswordResult.Succeeded)
                {
                    return View(user);
                }
                return View("BadRequest");
            }
            return View(updatedUser);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
            var currentUserId = currentUser.Id;

            var user = _userRepository.getUserById(currentUserId);

            if(user != null)
            {
                return View(user);
            }
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}