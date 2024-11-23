using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public ApplicationUser getUserById(string id)
        {
            var user = _applicationDbContext.applicationUsers.FirstOrDefault(u => u.Id == id);
            if (user != null)
                return user;
            else
                throw new Exception("Couldn't find user");
        }

        public ApplicationUser updateUser(ApplicationUser user)
        {
            var toUpdate = this.getUserById(user.Id);
            if (toUpdate != null)
            {
                toUpdate.ApplicationUserName = user.ApplicationUserName;
                toUpdate.Email = user.Email;
                toUpdate.registeredSince = user.registeredSince;
                _applicationDbContext.SaveChanges();
                return toUpdate;    
            }
            else
                throw new Exception("Couldn't find the user");
        }
    }
}
