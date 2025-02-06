using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;

namespace ExpenseTrackerApp.UnitTests.Repositories
{
    public class BudgetRepositoryTests
    {
        private Mock<ITransactionRepository> transactionRepositoryMock = new Mock<ITransactionRepository>();
        private Mock<ICategoryRepository> categoryRepositoryMock = new Mock<ICategoryRepository>();
        private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }
        
        // AddBudgetData Start
        [Fact]
        public void addBudgetDataSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetsWithAllRelations(context, 100);

            IEnumerable<SelectListItem> mockCategories = new List<SelectListItem>
            {
                new SelectListItem { Text = "Category1", Value = "1" },
                new SelectListItem { Text = "Category2", Value = "2" }
            };

            categoryRepositoryMock.Setup(repo => repo.GetAllCategoriesAsSelectListItems(user.Id))
                .Returns(mockCategories);

            var repository = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );

            // Act
            var result = repository.addBudgetData(user.Id);

            // Assert
            Assert.NotNull(result);
            result.Categories.Should().BeEquivalentTo(mockCategories);
            result.Budgets.Should().BeEquivalentTo(budgets);
        }

        [Fact]
        public void addBudgetDataSuccessNoDataTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            ApplicationUser user = AddDummyBudgetData.AddUser(context);

            var budgetRep = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );
            
            // Act
            var result = budgetRep.addBudgetData(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Budgets.Count);
            Assert.Equal(0, result.Categories.Count());
        }
        // AddBudgetData end
        
        // Create Budget Start
        [Fact]
        public void createBudgetSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            
            var mockCatgory = new Category() { Id=1, Title="Category1", ApplicationUserId = "1"};
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(mockCatgory);
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);
            
            // Act
            var budget = new Budget()
            {
                CategoryId = 1,
                Category = mockCatgory,
                Amount = 100,
            };
            
            budgetRepo.createBudget(budget);

            // Assert
            var savedBudget = context.budgets.FirstOrDefault(b => b.Id == budget.Id);
            Assert.NotNull(savedBudget);
            Assert.Equal(budget.Id, savedBudget.Id);
        }

        [Fact]
        public void createBudgetFailTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );
            
            // Act
            var exception = Assert.Throws<NullReferenceException>(() => budgetRepo.createBudget(null));

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }

        [Fact]
        public void createBudgetFailCategoryNullTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository
            (
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );

            // Act
            var budget = new Budget() { Id = 1, Amount = 100, Category = null };
            var exception = Assert.Throws<Exception>(() => budgetRepo.createBudget(budget));
            
            // Assert
            Assert.Equal("Categorie not found.", exception.Message);
        }
        // Create Budget End
        
        // Delete Budget Start
        [Fact]
        public void deleteBudgetSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var budget = AddDummyBudgetData.AddOneBudget(context, 0);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context
                );
            
            // Act
            budgetRepo.deleteBudget(budget.Id);
            
            // Assert
            var savedBudget = context.budgets.FirstOrDefault(b => b.Id == budget.Id);
            Assert.Null(savedBudget);
        }

        [Fact]
        public void deleteBudgetFailTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );
            
            // Act
            var exception = Assert.Throws<Exception>(() => budgetRepo.deleteBudget(1));
            
            // Assert
            Assert.Equal("Budget not found.", exception.Message);
        }
        // Delete Budget End
        
        // Update Budget Start
        [Fact]
        public void updateBudgetSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var budget = AddDummyBudgetData.AddOneBudget(context, 200);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );
            
            var updatedBudget = new Budget() { Id = 1, Amount = 200, CategoryId = 1 };
            
            // Act
            budgetRepo.updateBudget(updatedBudget);
            
            // Assert
            var savedBudget = context.budgets.FirstOrDefault(b => b.Id == budget.Id);
            Assert.NotNull(savedBudget);
            Assert.Equal(updatedBudget.Amount, savedBudget.Amount);
        }
        
        [Fact]
        public void updateBudgetFailBudgetNullTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            
            categoryRepositoryMock.Setup(repo => repo.findCategory(1)).Returns(new Category());
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );
            
            // Act
            var exception = Assert.Throws<NullReferenceException>(() => budgetRepo.updateBudget(null));
            // Assert
            
            Assert.Equal("Budget cannot be null.", exception.Message);
        }
        // Update Budget End
        
        // GetBudgetViewModelAsync Start
        [Fact]
        public void getBudgetViewModelAsyncOneBudgetAndZeroValuesSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetWithAllRelations(context, 0);

            transactionRepositoryMock.Setup(t => 
                t.GetSpendAmountForCertainCategoryThisMonth(user.Id, budgets[0].CategoryId)).Returns(0);
            transactionRepositoryMock.Setup(t =>
                t.GetSpendForCertainCategoryLastMonth(user.Id, budgets[0].CategoryId)).Returns(0);
            transactionRepositoryMock.Setup(t => 
                t.GetSpendMonthlyAverageForCertainCategory(user.Id, budgets[0].CategoryId)).Returns(0);
            transactionRepositoryMock.Setup(t =>
                t.GetExpensesForAllMonthsForCertainCategory(user.Id, budgets[0].CategoryId)).Returns(new List<decimal> ());
                
            var budgetRepo = new BudgetRepository(transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);
            // Act
            var result = budgetRepo.GetBudgetViewModelAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Budgets.Count);
            Assert.Equal(0, result.Result.Budgets[0].Budget.Amount);
            Assert.Equal(0, result.Result.Budgets[0].BudgetAmount);
            Assert.Equal(0, result.Result.Budgets[0].SpendAmount);
            Assert.Equal(0, result.Result.Budgets[0].SpendPercentage);
            Assert.Equal(0, result.Result.Budgets[0].SpendThisMonth);
            Assert.Equal(0, result.Result.Budgets[0].SpendLastMonth);
            Assert.Equal(0, result.Result.Budgets[0].SpendMonthlyAverage);
            Assert.Equal(12, result.Result.Budgets[0].BudgetsForYear.Count);
            Assert.Equal(0, result.Result.Budgets[0].ExpensesForYear.Count);
        }
        
        [Fact]
        public void getBudgetViewModelAsyncOneBudgetValuesSpend50PercentOfBudgetSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetWithAllRelations(context, 100);

            transactionRepositoryMock.Setup(t => 
                t.GetSpendAmountForCertainCategoryThisMonth(user.Id, budgets[0].CategoryId)).Returns(50);
            transactionRepositoryMock.Setup(t =>
                t.GetSpendForCertainCategoryLastMonth(user.Id, budgets[0].CategoryId)).Returns(50);
            transactionRepositoryMock.Setup(t => 
                t.GetSpendMonthlyAverageForCertainCategory(user.Id, budgets[0].CategoryId)).Returns((decimal)8.33);
            transactionRepositoryMock.Setup(t =>
                t.GetExpensesForAllMonthsForCertainCategory(user.Id, budgets[0].CategoryId)).Returns(new List<decimal> ()
            {
                50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50
            });
                
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);
            // Act
            var result = budgetRepo.GetBudgetViewModelAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Budgets.Count);
            Assert.Equal(100, result.Result.Budgets[0].Budget.Amount);
            Assert.Equal(100, result.Result.Budgets[0].BudgetAmount);
            Assert.Equal(50, result.Result.Budgets[0].SpendAmount);
            Assert.Equal(50, result.Result.Budgets[0].SpendPercentage);
            Assert.Equal(50, result.Result.Budgets[0].SpendThisMonth);
            Assert.Equal(50, result.Result.Budgets[0].SpendLastMonth);
            Assert.Equal((decimal)8.33, result.Result.Budgets[0].SpendMonthlyAverage);
            Assert.Equal(12, result.Result.Budgets[0].BudgetsForYear.Count);
            Assert.Equal(12, result.Result.Budgets[0].ExpensesForYear.Count);
        }
        
        [Fact]
        public void getBudgetViewModelAsyncOneBudgetRealisticValuesPercentOfBudgetSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetWithAllRelations(context, 1000);

            transactionRepositoryMock.Setup(t => 
                t.GetSpendAmountForCertainCategoryThisMonth(user.Id, budgets[0].CategoryId)).Returns(200);
            transactionRepositoryMock.Setup(t =>
                t.GetSpendForCertainCategoryLastMonth(user.Id, budgets[0].CategoryId)).Returns(0);
            transactionRepositoryMock.Setup(t => 
                t.GetSpendMonthlyAverageForCertainCategory(user.Id, budgets[0].CategoryId)).Returns((decimal)16.66);
            transactionRepositoryMock.Setup(t =>
                t.GetExpensesForAllMonthsForCertainCategory(user.Id, budgets[0].CategoryId)).Returns(new List<decimal> ()
            {
                50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50
            });
                
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);
            // Act
            var result = budgetRepo.GetBudgetViewModelAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Budgets.Count);
            Assert.Equal(1000, result.Result.Budgets[0].Budget.Amount);
            Assert.Equal(1000, result.Result.Budgets[0].BudgetAmount);
            Assert.Equal(200, result.Result.Budgets[0].SpendAmount);
            Assert.Equal(20, result.Result.Budgets[0].SpendPercentage);
            Assert.Equal(200, result.Result.Budgets[0].SpendThisMonth);
            Assert.Equal(0, result.Result.Budgets[0].SpendLastMonth);
            Assert.Equal((decimal)16.66, result.Result.Budgets[0].SpendMonthlyAverage);
            Assert.Equal(12, result.Result.Budgets[0].BudgetsForYear.Count);
            Assert.Equal(12, result.Result.Budgets[0].ExpensesForYear.Count);
        }
        
        // GetBudgetViewModelAsync Start

        [Fact]
        public void getBudgetViewModelAsyncSuccessNoDataTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var user = AddDummyBudgetData.AddUser(context);

            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );

            // Act
            var result = budgetRepo.GetBudgetViewModelAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Result.Budgets.Count);
        }

        [Fact]
        public void getBudgetViewModelAsyncFailTest()
        {
            
        }
        // GetBudgetViewModelAsync End
        
        // GetAllBudgets Start
        [Fact]
        public void getAllBudgetsSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets)  = AddDummyBudgetData.AddBudgetsWithAllRelations(context,0);

            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context);

            // Act
            Task<List<Budget>> result = budgetRepo.GetAllBudgets(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(budgets.Count, result.Result.Count());
        }

        [Fact]
        public void getAllBudgetsNotEntriesTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var user = AddDummyBudgetData.AddUser(context);

            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context);

            // Act
            Task<List<Budget>> result = budgetRepo.GetAllBudgets(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Result.Count());
        }
        // GetAllBudgets End
        
        // GetSumOfAllBudgets Start
        [Fact]
        public void getSumOfAllBudgetsSuccessTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetsWithAllRelations(context, 100);
            
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);

            // Act
            var result = budgetRepo.GetSumOfAllBudgets(user.Id);

            // Assert
            Assert.Equal(200, result);
        }

        [Fact]
        public void getSumOfAllBudgetsIsZeroTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var (user, budgets) = AddDummyBudgetData.AddBudgetsWithAllRelations(context, 0);

            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);
            
            // Act
            var result = budgetRepo.GetSumOfAllBudgets(user.Id);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void getSumOfAllBudgetsIsNoEntriesTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);
            var user = AddDummyBudgetData.AddUser(context);
            
            var budgetRepo = new BudgetRepository(
                transactionRepositoryMock.Object, 
                categoryRepositoryMock.Object, 
                context);

            // Act
            var result = budgetRepo.GetSumOfAllBudgets(user.Id);

            // Assert
            Assert.Equal(0, result);
        }
        // GetSumOfAllBudgets End

        // Test FindBudget Start
        [Fact]
        public void findBudgetSuccessTest()
        {
            // Db
            var options = CreateDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var budget = AddDummyBudgetData.AddOneBudgetWithAllRelations(context);

            // Arrange
            var budgetRepository = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );

            // Act
            var result = budgetRepository.findBudget(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(budget, result);
        }

        [Fact]
        public void findBudgetFailTest()
        {
            // Arrange
            var options = CreateDbContextOptions();
            using var context = new ApplicationDbContext(options);

            var budgetRepository = new BudgetRepository
                (
                    transactionRepositoryMock.Object,
                    categoryRepositoryMock.Object,
                    context
                );

            // Act
            var exception = Assert.Throws<Exception>(() => budgetRepository.findBudget(1));

            // Assert
            Assert.Equal("Couldn't find any budget", exception.Message);
        }
        // Test find Budget end
    }
}
