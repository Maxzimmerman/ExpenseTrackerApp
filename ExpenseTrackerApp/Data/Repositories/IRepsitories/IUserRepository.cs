using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IUserRepository : IRepository<ApplicationUser> 
    {
        ApplicationUser? getUserById(string id);
        ApplicationUser updateUser(ApplicationUser user);
        ApplicationUser findByEmail(string email);
        ProfileViewModel getProfileData(string userId);
    }
}
