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

            // DataBase Start
            var options = CreateDbContextOptions();
            var context = new ApplicationDbContext(options);

            // DataBase End

            // Providing Data Start
            var userId = "123";

            IEnumerable<SelectListItem> mockCategories = new List<SelectListItem>
            {
                new SelectListItem { Text = "Category1", Value = "1" },
                new SelectListItem { Text = "Category2", Value = "2" }
            };

            var categoryColor = new CategoryColor { Id = 1, code = "code1", Name = "Red" };
            var categoryIcon = new CategoryIcon { Id = 1, Code = "icon1", Name = "Book" };

            var categories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Title = "Budget1",
                    ApplicationUserId = userId,
                    CategoryColor = categoryColor,
                    CategoryIcon = categoryIcon
                },
            };

            var budgets = new List<Budget>
            {
                new Budget { Amount = 100, Category = categories[0] }
            };
            
            

            context.categoriesColors.Add(categoryColor);
            context.categoriesIcons.Add(categoryIcon);
            context.categories.AddRange(categories);
            context.budgets.AddRange(budgets);
            context.SaveChanges();

            categoryRepositoryMock.Setup(repo => repo.GetAllCategoriesAsSelectListItems(userId))
                .Returns(mockCategories);

            var repository = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context
            );

            // Act
            var result = repository.addBudgetData("123");

            // Assert
            result.Categories.Should().BeEquivalentTo(mockCategories);
            result.Budgets.Should().BeEquivalentTo(budgets);
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
        
        // Create Budget End

        // Test FindBudget Start
        [Fact]
        public void findBudgetSuccessTest()
        {
            // Db
            var options = CreateDbContextOptions();
            using var context = new ApplicationDbContext(options);

            // Arrange
            var budget = new Budget
            {
                Id = 1,
                Amount = 100,
                Category = new Category
                {
                    Title = "Category1",
                    ApplicationUserId = "user1",
                    CategoryColor = new CategoryColor { Id = 1, code = "code1", Name = "name1" },
                    CategoryIcon = new CategoryIcon { Id = 1, Code = "code1", Name = "name1" },
                    CategoryType = new CategoryType { Id = 1, Name = "name1" },
                    ApplicationUser = new ApplicationUser { Balance = 1, ApplicationUserName = "user1" }
                }
            };

            context.budgets.Add(budget);
            context.SaveChanges();

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
