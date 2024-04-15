using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IUserRepository
    {
        ApplicationUser getUserById(string id);
        ApplicationUser updateUser(ApplicationUser user);
    }
}
