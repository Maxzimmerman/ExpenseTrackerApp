using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Data;
using Microsoft.AspNetCore.Authorization;
using ExpenseTrackerApp.Models.ViewModels.UserViewModels;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBudgetRepository _budgetRepository;

        public UserRepository(ApplicationDbContext applicationDbContext,
            ITransactionRepository transactionRepository,
            IBudgetRepository budgetRepository) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
        }

        public ApplicationUser? findByEmail(string email)
        {
            return _applicationDbContext.applicationUsers.FirstOrDefault(u => u.Email == email);
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

        // Pages
        public ProfileViewModel getProfileData(string userId)
        {
            ProfileViewModel profileViewModel = new ProfileViewModel();
            profileViewModel.TotalSpend = _transactionRepository.GetTotalSpendAmount(userId);
            try
            {
                profileViewModel.TotalBudget = _budgetRepository.GetSumOfAllBudgets(userId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            double spendPercentage = 0;
            if(profileViewModel.TotalBudget > 0)
                spendPercentage = Math.Round((double)(profileViewModel.TotalSpend / profileViewModel.TotalBudget) * 100, 2);
            
            if (spendPercentage > 100)
                profileViewModel.SpendPercentage = 100;
            else
                profileViewModel.SpendPercentage = spendPercentage;
            return profileViewModel;
        }
    }
}
