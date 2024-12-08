using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class UserManage : BaseController
    {
        private readonly IUserManageService _userManageService;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public UserManage(
            IUserManageService userManageService,
            IFooterRepository footerRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository) : base(footerRepository)
        {
            _userManageService = userManageService;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
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
            // check if user already exists
            var existingUser = _userRepository.findByEmail(signUpviewModel.Email);

            if (existingUser != null)
            {
                return RedirectToAction("UserAlreadyExists", "UserManage");
            }

            // sign up user
            if (ModelState.IsValid)
            {
                var user = await _userManageService.SignUp(signUpviewModel);
                _messageRepository.CreateMessageWithUserId(
                    user.Id, 
                    "Account created successfully", 
                    "success", "fi-bs-check", 
                    "UserManage", 
                    "SettingsProfile"
                    );
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

                if (Microsoft.AspNetCore.Identity.SignInResult.Success == await _userManageService.SignIn(signInviewModel))
                {
                    return RedirectToAction("Home", "Home");
                }
                return RedirectToAction("AccessDenied", "UserManage");
            }
            return RedirectToAction("AccessDenied", "UserManage");
        }

        [Authorize]
        public IActionResult SignOut()
        {
            _userManageService.SignOut();
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
                ProfileViewModel profileViewModel = new ProfileViewModel();
                var profileData = _userRepository.getProfileData(user.Id);
                profileViewModel = profileData;
                profileViewModel.User = user;
                return View(profileViewModel);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (await _userManageService.ConfirmEmail(userId, token) == IdentityResult.Success)
                return RedirectToAction("SignIn", "UserManage");
            return View("Error");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(SendResetEmailModelView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userRepository.findByEmail(model.EmailAddress);
            if (user == null)
            {
                return RedirectToAction("UserWasNotFound", "UserManage");
            }

            var resetToken = await _userManageService.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword",
                "UserManage",
                new { token = resetToken, email = model.EmailAddress },
                Request.Scheme);

            await _userManageService.SendPasswordResetEmail(model.EmailAddress, resetLink);

            return RedirectToAction("ResetEmailHasBeenSend", "UserManage");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Error");
            }

            var model = new ResetPasswordViewModel { Token = token, EmailAdress = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userRepository.findByEmail(model.EmailAdress);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email address.");
                return View(model);
            }

            var result = await _userManageService.ResetPasswordAsync(model.EmailAdress, model.Token, model.Password);
            if (result.Succeeded)
            {
                _messageRepository.CreateMessageWithUserId(
                    user.Id, 
                    "Changes password successfully", 
                    "success", 
                    "fi-bs-check", 
                    "UserManage",
                    "SettingsProfile"
                    );
                return RedirectToAction("SignIn", "UserManage");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetEmailHasBeenSend()
        {
            return View();
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

        [HttpGet]
        public IActionResult UserWasNotFound()
        {
            return View();
        }
    }
}