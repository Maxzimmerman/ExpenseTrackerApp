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
                    Title="Category1", 
                    ApplicationUserId="user1",  
                    CategoryColor=new CategoryColor { Id=1, code="code1", Name="name1"},
                    CategoryIcon=new CategoryIcon { Id=1, Code="code1", Name="name1"},
                    CategoryType=new CategoryType { Id=1, Name="name1"},
                    ApplicationUser=new ApplicationUser { Balance=1, ApplicationUserName="user1"}
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

        [Fact]
        public void addBudgetDataSuccessTest()
        {
            // Arrange

            // DataBase Start
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            var context = new ApplicationDbContext(options);

            // DataBase End

            // Providing Data Start
            var userId = "6b92202f-0092-40ba-9a04-d96f55c89548";

            IEnumerable<SelectListItem> mockCategories = new List<SelectListItem>
            {
                new SelectListItem { Text = "Category1", Value = "1" },
                new SelectListItem { Text = "Category2", Value = "2" }
            };

            var category1 = new Category { Id = 1, Title = "Budget1", ApplicationUserId = userId };
            var category2 = new Category { Id = 2, Title = "Budget2", ApplicationUserId = userId };

            var mockBudgets = new List<Budget>
            {
                new Budget { Id = 1, Amount = 100, Category = category1, CategoryId = category1.Id },
                new Budget { Id = 2, Amount = 200, Category = category2, CategoryId = category2.Id }
            };

            // Seed categories and budgets only once
            context.categories.AddRange(category1, category2);
            context.budgets.AddRange(mockBudgets);
            context.SaveChanges();

            categoryRepositoryMock.Setup(repo => repo.GetAllCategoriesAsSelectListItems(userId))
                .Returns(mockCategories);

            var repository = new BudgetRepository(
                transactionRepositoryMock.Object,
                categoryRepositoryMock.Object,
                context // Reuse the same context
            );

            // Act
            var result = repository.addBudgetData(userId);

            // Assert
            result.Categories.Should().BeEquivalentTo(mockCategories);
            result.Budgets.Should().BeEquivalentTo(mockBudgets, options => options.IncludingNestedObjects());
        }
    }
}
