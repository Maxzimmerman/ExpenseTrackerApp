using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExpenseTrackerApp.Services.IServices
{
    public interface IUserManageService
    {
        Task<SignInResult> SignIn(SignInviewModel signInviewModel);
        void SignOut();
        Task<ApplicationUser> SignUp(SignUpviewModel signUpviewModel);
        Task<IdentityResult> ConfirmEmail(string userId, string token);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal user);
        string GetCurrentUserId(ClaimsPrincipal user);
        Task<ApplicationUser> UpdateUser(ApplicationUser user, string currentPassword);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
        Task SendPasswordResetEmail(string email, string resetLink);
    }
}
