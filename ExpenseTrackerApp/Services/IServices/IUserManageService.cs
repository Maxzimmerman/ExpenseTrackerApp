using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using System.Security.Claims;

namespace ExpenseTrackerApp.Services.IServices
{
    public interface IUserManageService
    {
        Task<bool> SignIn(SignInviewModel signInviewModel);
        Task<bool> SignOut();
        Task<ApplicationUser> SignUp(SignUpviewModel signUpviewModel);
        Task<bool> ConfirmEmail(string userId, string token);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal user);
        string GetCurrentUserId(ClaimsPrincipal user);
        Task<ApplicationUser> UpdateUser(ApplicationUser user, string currentPassword);
    }
}
