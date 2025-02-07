using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;
using Moq;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class TransactionRepositoryTests
{
    private Mock<ICategoryRepository> mockCategoryRepository = new Mock<ICategoryRepository>();
    private Mock<IBudgetRepository> mockBudgetRepository = new Mock<IBudgetRepository>();
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    // get balance trends data start
    [Fact]
    public void getBalanceTrendsDataSuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

            CategoryType expenseType = new CategoryType() { Name = "Expense" };
            CategoryType incomeType = new CategoryType() { Name = "Income" };
            CategoryIcon icon = new CategoryIcon() { Name = "Icon", Code = "icon" };
            CategoryColor color = new CategoryColor() { Name = "Color", code = "color" };
            ApplicationUser user = new ApplicationUser() { ApplicationUserName = "user" };
            
            Category expenseCategory = new Category()
            {
                Title = "Groceries",
                CategoryType = expenseType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };

            Category incomeCategory = new Category()
            {
                Title = "Job",
                CategoryType = incomeType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };
            
            var transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Title = "Test1", 
                    Description = "Test1", 
                    Date = new DateTime(2025, 01, 01), 
                    Amount = 10000,
                    Category = incomeCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 500,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 02, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 03, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 04, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 05, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 06, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 07, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 08, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 09, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 10, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 11, 01),
                    Amount = 50,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 12, 01),
                    Amount = 1000,
                    Category = incomeCategory,
                    ApplicationUser = user,
                }
            };

            context.Add(expenseType);
            context.Add(incomeType);
            context.Add(icon);
            context.Add(color);
            context.Add(user);
            context.Add(expenseCategory);
            context.Add(incomeCategory);
            context.AddRange(transactions);
            context.SaveChanges();

            var expectedBalances = new List<decimal>()
            {
                9500,
                9450,
                9400,
                9350,
                9300,
                9250,
                9200,
                9150,
                9100,
                9050,
                9000,
                10000
            };
            
            var expectedAverageLastMonth = expectedBalances[DateTime.Now.Month - 1];
            if(expectedAverageLastMonth < 0)
                expectedAverageLastMonth *= -1;
            var expectedPercentage = ((expectedBalances[DateTime.UtcNow.Month] - expectedAverageLastMonth) / expectedAverageLastMonth) * 100;
            
            // Act
            var result = transactionRepository.getBalanceTrendsData(user.Id);

            // Assert
            result.Balance.Should().Be(10000);
            result.Balances.Should().BeEquivalentTo(expectedBalances);
            result.BalancePercentage.Should().Be(Math.Round(expectedPercentage, 2));
        }
    }

    [Fact]
    public void getBalanceTrendsDataNoEntriesInDb()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);
            
            // Act
            var result = transactionRepository.getBalanceTrendsData("userId");
            
            // Assert
            result.Balance.Should().Be(0);
            result.Balances.Should().BeEquivalentTo(new List<decimal>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            result.BalancePercentage.Should().Be(0);
        }
    }
    
    // get balance trends data end

    [Fact]
    public void getMonthlyBalanceAverageForCertainMonthThisYear()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);
            
            CategoryType expenseType = new CategoryType() { Name = "Expense" };
            CategoryType incomeType = new CategoryType() { Name = "Income" };
            CategoryIcon icon = new CategoryIcon() { Name = "Icon", Code = "icon" };
            CategoryColor color = new CategoryColor() { Name = "Color", code = "color" };
            ApplicationUser user = new ApplicationUser() { ApplicationUserName = "user" };
            
            Category expenseCategory = new Category()
            {
                Title = "Groceries",
                CategoryType = expenseType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };

            Category incomeCategory = new Category()
            {
                Title = "Job",
                CategoryType = incomeType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };
            
            var transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Title = "Test1", 
                    Description = "Test1", 
                    Date = new DateTime(2025, 01, 01), 
                    Amount = 19820,
                    Category = incomeCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 890,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 30,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 10,
                    Category = expenseCategory,
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 600,
                    Category = incomeCategory,
                    ApplicationUser = user,
                }
            };

            context.Add(expenseType);
            context.Add(incomeType);
            context.Add(icon);
            context.Add(color);
            context.Add(user);
            context.Add(expenseCategory);
            context.Add(incomeCategory);
            context.AddRange(transactions);
            context.SaveChanges();

            var expectedBalance = 19490;
            
            // Act
            var result = transactionRepository.getMonthlyBalanceAverageForCertainMonthThisYear(user.Id, 1);
            
            // Assert
            result.Should().Be(expectedBalance);
        }
    }

    [Fact]
    public void getMonthlyBalanceAverageForCertainMonthThisYearNegativeBalance()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

            CategoryType expenseType = new CategoryType() { Name = "Expense" };
            CategoryType incomeType = new CategoryType() { Name = "Income" };
            CategoryIcon icon = new CategoryIcon() { Name = "Icon", Code = "icon" };
            CategoryColor color = new CategoryColor() { Name = "Color", code = "color" };
            ApplicationUser user = new ApplicationUser() { ApplicationUserName = "user" };

            Category expenseCategory = new Category()
            {
                Title = "Groceries",
                CategoryType = expenseType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };

            Category incomeCategory = new Category()
            {
                Title = "Job",
                CategoryType = incomeType,
                CategoryIcon = icon,
                CategoryColor = color,
                ApplicationUser = user
            };

            var transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Title = "Test1",
                    Description = "Test1",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 19,
                    Category = expenseCategory,
                    ApplicationUser = user,
                }
            };

            context.Add(expenseType);
            context.Add(incomeType);
            context.Add(icon);
            context.Add(color);
            context.Add(user);
            context.Add(expenseCategory);
            context.Add(incomeCategory);
            context.AddRange(transactions);
            context.SaveChanges();

            var expectedBalance = -19;

            // Act
            var result = transactionRepository.getMonthlyBalanceAverageForCertainMonthThisYear(user.Id, 1);

            // Assert
            result.Should().Be(expectedBalance);
        }
    }

    [Fact]
    public void getMonthlyBalanceAverageForCertainMonthThisYearNoEntriesInDb()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

            var expectedBalance = 0;
            
            // Act
            var result = transactionRepository.getMonthlyBalanceAverageForCertainMonthThisYear("user.Id", 1);
            
            // Assert
            result.Should().Be(expectedBalance);
        }
    }
}