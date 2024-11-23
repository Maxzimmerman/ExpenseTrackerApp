using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ExpenseTrackerApp.Services
{
    public class UserManageService : IUserManageService
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UserManageService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("User ID claim not found.");
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            return _userRepository.getUserById(currentUser.Id);
        }

        public async Task<bool> SignIn(SignInviewModel signInviewModel)
        {
            var result = await _signInManager.PasswordSignInAsync(signInviewModel.Email, signInviewModel.Password, signInviewModel.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SignOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SignUp(SignUpviewModel signUpviewModel)
        {
            var user = new ApplicationUser { UserName = signUpviewModel.Email, Email = signUpviewModel.Email, ApplicationUserName = signUpviewModel.Name, registeredSince = DateTime.Today };
            var result = await _userManager.CreateAsync(user, signUpviewModel.Password);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
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
    }
}
