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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser getUserById(string id)
        {
            var user = _context.applicationUsers.FirstOrDefault(u => u.Id == id);
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
                _context.SaveChanges();
                return toUpdate;    
            }
            else
                throw new Exception("Couldn't find the user");
        }
    }
}
