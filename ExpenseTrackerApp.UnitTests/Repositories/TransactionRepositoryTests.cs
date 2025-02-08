using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;
using Moq;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class TransactionRepositoryTests
{
    private Mock<ICategoryRepository> mockCategoryRepository = new Mock<ICategoryRepository>();
    private Mock<Lazy<IBudgetRepository>> mockBudgetRepository = new Mock<Lazy<IBudgetRepository>>();

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
            var transactionRepository =
                new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

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
            if (expectedAverageLastMonth < 0)
                expectedAverageLastMonth *= -1;
            var expectedPercentage = ((expectedBalances[DateTime.UtcNow.Month] - expectedAverageLastMonth) /
                                      expectedAverageLastMonth) * 100;

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
            var transactionRepository =
                new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

            // Act
            var result = transactionRepository.getBalanceTrendsData("userId");

            // Assert
            result.Balance.Should().Be(0);
            result.Balances.Should().BeEquivalentTo(new List<decimal>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            result.BalancePercentage.Should().Be(0);
        }
    }

    // get balance trends data end

    // get monthly balance average for certain month this year start
    [Fact]
    public void getMonthlyBalanceAverageForCertainMonthThisYear()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository =
                new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

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
            var transactionRepository =
                new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

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
            var transactionRepository =
                new TransactionRepository(context, mockCategoryRepository.Object, mockBudgetRepository.Object);

            var expectedBalance = 0;

            // Act
            var result = transactionRepository.getMonthlyBalanceAverageForCertainMonthThisYear("user.Id", 1);

            // Assert
            result.Should().Be(expectedBalance);
        }
    }

    // get monthly balance average for certain month this year end

    // get monthly budget data start
    [Fact]
    public void getMonthlyBudgetDataSuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();

        using (var context = new ApplicationDbContext(options))
        {
            var user = new ApplicationUser() { ApplicationUserName = "user" };
            CategoryType expenseType = new CategoryType() { Name = "Expense" };
            CategoryIcon icon = new CategoryIcon() { Name = "Icon", Code = "icon" };
            CategoryColor color = new CategoryColor() { Name = "Color", code = "color" };
            var categories = new List<Category>()
            {
                new Category()
                {
                    Title = "Groceries",
                    CategoryType = expenseType,
                    CategoryIcon = icon,
                    CategoryColor = color,
                    ApplicationUser = user
                },
                new Category()
                {
                    Title = "Casino",
                    CategoryType = expenseType,
                    CategoryIcon = icon,
                    CategoryColor = color,
                    ApplicationUser = user
                },
                new Category()
                {
                    Title = "Hobby",
                    CategoryType = expenseType,
                    CategoryIcon = icon,
                    CategoryColor = color,
                    ApplicationUser = user
                }
            };

            var transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Title = "Test1",
                    Description = "Test1",
                    Date = DateTime.UtcNow,
                    Amount = 20,
                    Category = categories[0],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = DateTime.UtcNow,
                    Amount = 90,
                    Category = categories[1],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = DateTime.UtcNow,
                    Amount = 30,
                    Category = categories[0],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = new DateTime(2025, 01, 01),
                    Amount = 10,
                    Category = categories[1],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = DateTime.UtcNow,
                    Amount = 600,
                    Category = categories[0],
                    ApplicationUser = user,
                }
            };

            var budgets = new List<Budget>
            {
                new Budget() { Amount = 100, Category = categories[0] },
                new Budget() { Amount = 800, Category = categories[1] },
                new Budget() { Amount = 1, Category = categories[2] }
            };

            context.Add(expenseType);
            context.Add(icon);
            context.Add(color);
            context.Add(user);
            context.AddRange(categories);
            context.AddRange(budgets);
            context.AddRange(transactions);
            context.SaveChanges();

            var mockBudgetRepository = new Mock<IBudgetRepository>();

            mockBudgetRepository.Setup(repository => repository.GetAllBudgets(user.Id))
                .Returns(Task.FromResult(budgets));

            var lazyMockBudgetRepository = new Lazy<IBudgetRepository>(() => mockBudgetRepository.Object);

            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                lazyMockBudgetRepository);

            // Act
            var result = transactionRepository.getMonthlyBudgetData(user.Id);

            var expectedResult = new List<MonthyBudgetEntryViewModel>()
            {
                new MonthyBudgetEntryViewModel()
                {
                    Name = "Groceries",
                    BudgetAmount = 100,
                    SpendAmount = 650,
                    SpendPercentage = 100
                },
                new MonthyBudgetEntryViewModel()
                {
                    Name = "Casino",
                    BudgetAmount = 800,
                    SpendAmount = 90,
                    SpendPercentage = (decimal)11.25
                },
                new MonthyBudgetEntryViewModel()
                {
                    Name = "Hobby",
                    BudgetAmount = 1,
                    SpendAmount = 0,
                    SpendPercentage = 0
                }
            };

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
    
    [Fact]
    public void getMonthlyBudgetDataSuccessNoData()
    {
        // Arrange
        var options = CreateDbContextOptions();

        using (var context = new ApplicationDbContext(options))
        {
            var user = new ApplicationUser() { ApplicationUserName = "user" };
            
            var mockBudgetRepository = new Mock<IBudgetRepository>();

            mockBudgetRepository.Setup(repository => repository.GetAllBudgets(user.Id))
                .Returns(Task.FromResult(new List<Budget>()));

            var lazyMockBudgetRepository = new Lazy<IBudgetRepository>(() => mockBudgetRepository.Object);

            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                lazyMockBudgetRepository);

            // Act
            var result = transactionRepository.getMonthlyBudgetData(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    // get monthly budget data end
}