using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;


namespace ExpenseTrackerApp.Services
{
    public class UserManageService : IUserManageService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IServices.IEmailSender _emailSender;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public UserManageService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            IServices.IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found or the token was not provided" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<ApplicationUser?> GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("User ID claim not found.");
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            return _userRepository.getUserById(currentUser.Id);
        }

        public string GetCurrentUserId(ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated)
                throw new InvalidOperationException("User is not authenticated.");

            var userId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("User ID claim not found.");
            }
            return userId;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(SignInviewModel signInviewModel)
        {
            return await _signInManager.PasswordSignInAsync(signInviewModel.Email, signInviewModel.Password, signInviewModel.RememberMe, lockoutOnFailure: false);
        }

        public async void SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> SignUp(SignUpviewModel signUpviewModel)
        {
            // create user
            var user = new ApplicationUser
            {
                UserName = signUpviewModel.Email,
                Email = signUpviewModel.Email,
                ApplicationUserName = signUpviewModel.Name,
                registeredSince = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, signUpviewModel.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // Use IHttpContextAccessor to construct the link
                var context = _httpContextAccessor.HttpContext;
                var urlHelper = _urlHelperFactory.GetUrlHelper(
                    new ActionContext(context, context.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));

                var confirmationLink = urlHelper.Action(
                    action: "ConfirmEmail",
                controller: "UserManage",
                    values: new { userId = user.Id, token = Uri.EscapeDataString(token) },
                    protocol: context.Request.Scheme);

                // Send confirmation email
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by clicking this link: {confirmationLink}");

                return user;
            }
            throw new Exception("Couldn't create user");
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user, string currentPassword)
        {
            var updatedUser = _userRepository.updateUser(user);

            if (!string.IsNullOrEmpty(currentPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(updatedUser);
                var changePasswordResult = await _userManager.ResetPasswordAsync(updatedUser, token, currentPassword);

                if (changePasswordResult.Succeeded)
                {
                    return user;
                }
                throw new Exception("Coudn't change password");
            }
            return updatedUser;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var subject = "Reset your password";
            var body = $"Click {resetLink} to reset your password.";
            await _emailSender.SendEmailAsync(email, subject, body);
        }
    }
}
