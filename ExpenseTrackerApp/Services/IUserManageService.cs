using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using System.Security.Claims;

namespace ExpenseTrackerApp.Services
{
    public interface IUserManageService
    {
        Task<bool> SignIn(SignInviewModel signInviewModel);
        Task<bool> SignOut();
        Task<bool> SignUp(SignUpviewModel signUpviewModel);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal user);
        Task<ApplicationUser> UpdateUser(ApplicationUser user, string currentPassword);
    }
}
