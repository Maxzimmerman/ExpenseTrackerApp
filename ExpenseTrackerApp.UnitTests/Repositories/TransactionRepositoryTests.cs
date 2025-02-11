using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;
using Moq;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

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
            var result = transactionRepository.getMonthlyBalanceForCertainMonthThisYear(user.Id, 1);

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
            var result = transactionRepository.getMonthlyBalanceForCertainMonthThisYear(user.Id, 1);

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
            var result = transactionRepository.getMonthlyBalanceForCertainMonthThisYear("user.Id", 1);

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

    // get Percentage Of Transaction Of CertainCategory This Month start
    [Fact]
    public void GetPercentageOfTransactionOfCertainCategoryThisMonthSuccessTest()
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
                    Amount = 600,
                    Category = categories[0],
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
                    Amount = 10,
                    Category = categories[1],
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

            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                mockBudgetRepository.Object
            );

            // Act
            var result =
                transactionRepository.GetPercentageOfTransactionOfCertainCategoryThisMonth(user.Id, "Expense",
                    categories[0].Id);

            // Assert
            result.Should().Be(87);
        }
    }

    [Fact]
    public void GetPercentageOfTransactionOfCertainCategoryThisMonthSuccessPercentageMoreThanHundredTest()
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
                    Amount = 200,
                    Category = categories[0],
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

            mockCategoryRepository.Setup(mc => mc.GetTotalAmountOfAllCategories(user.Id, "Expense"))
                .Returns(650);

            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                mockBudgetRepository.Object
            );

            // Act
            var result =
                transactionRepository.GetPercentageOfTransactionOfCertainCategoryThisMonth(user.Id, "Expense",
                    categories[0].Id);

            // Assert
            result.Should().Be(100);
        }
    }

    [Fact]
    public void GetPercentageOfTransactionOfCertainCategoryThisMonthSuccessNoDataTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                mockBudgetRepository.Object
            );

            // Act
            var result =
                transactionRepository.GetPercentageOfTransactionOfCertainCategoryThisMonth("userId", "Expense", 1);

            // Assert
            result.Should().Be(0);
        }
    }
    // get Percentage Of Transaction Of CertainCategory This Month end

    // get MonthlyExpense Breakdown start
    [Fact]
    public void getMonthlyExpenseBreakeDownSucessTest()
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
                    Category = categories[0],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = DateTime.UtcNow,
                    Amount = 30,
                    Category = categories[1],
                    ApplicationUser = user,
                },
                new Transaction()
                {
                    Title = "Test2",
                    Description = "Test2",
                    Date = DateTime.UtcNow,
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
                    Category = categories[2],
                    ApplicationUser = user,
                }
            };

            var budgets = new List<Budget>
            {
                new Budget() { Amount = 110, Category = categories[0] },
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

            mockCategoryRepository
                .Setup(repo => repo.GetAllExpenseCategories(user.Id))
                .Returns(categories);

            var transactionRepository = new TransactionRepository(
                context,
                mockCategoryRepository.Object,
                mockBudgetRepository.Object
            );

            var expectedResult = new List<ExpenseAndIncomeCategoryData>()
            {
                new ExpenseAndIncomeCategoryData()
                {
                    Title = "Groceries",
                    Amount = 110,
                    Percentage = 15
                },
                new ExpenseAndIncomeCategoryData()
                {
                    Title = "Casino",
                    Amount = 40,
                    Percentage = 5
                },
                new ExpenseAndIncomeCategoryData()
                {
                    Title = "Hobby",
                    Amount = 600,
                    Percentage = 80
                }
            };

            // Act
            var result = transactionRepository.getMonthlyExpenseBreakDown(user.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    [Fact]
    public void GetMonthlyExpenseBreakdown_NoData_ReturnsEmptyList()
    {
        // Arrange
        var options = CreateDbContextOptions();

        // Mock category repository to return an empty list instead of null
        mockCategoryRepository
            .Setup(repo => repo.GetAllExpenseCategories("userId"))
            .Returns(new List<Category>());

        using var context = new ApplicationDbContext(options);
        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        // Act
        var result = transactionRepository.getMonthlyExpenseBreakDown("userId");

        // Assert
        result.Should().BeEquivalentTo(new List<ExpenseAndIncomeCategoryData>());
    }
    // get Monthly Expense Breakdown end

    // Get Expense Total Amount For All Categories This Month start
    [Fact]
    public void GetExpenseTotalAmountForAllCategoriesThisMonthTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);

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
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
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
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );
        
        // Act
        var result = transactionRepository.GetExpenseTotalAmountForAllCategoriesThisMonth(user.Id);
        
        // Assert
        result.Should().Be(750);
    }
    
    [Fact]
    public void GetExpenseTotalAmountForAllCategoriesThisMonthNoDataTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );
        
        // Act
        var result = transactionRepository.GetExpenseTotalAmountForAllCategoriesThisMonth("userId");
        
        // Assert
        result.Should().Be(0);
    }
    // Get Expense Total Amount For All Categories This Month end
    
    // get total balance data start
    [Fact]
    public void getTotalBalanceDataSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
        var user = new ApplicationUser() { ApplicationUserName = "user" };
        CategoryType expenseType = new CategoryType() { Name = "Expense" };
        CategoryType incomeType = new CategoryType() { Name = "Income" };
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
                CategoryType = incomeType,
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
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 20,
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 90,
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
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
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalBalanceDataViewModel()
        {
            TotalBalance = 450,
            BalanceLastMonth = -110,
            DifferenceFromLastToCurrentMonthPercentage = (decimal)-509.09
        };
        
        // Act
        var result = transactionRepository.getTotalBalanceData(user.Id);
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public void getTotalBalanceDataSuccessBalanceLastMonthWaszeroTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
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
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
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
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalBalanceDataViewModel()
        {
            TotalBalance = -750,
            BalanceLastMonth = 0,
            DifferenceFromLastToCurrentMonthPercentage = 0
        };
        
        // Act
        var result = transactionRepository.getTotalBalanceData(user.Id);
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public void getTotalBalanceDataSuccessNoDataTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );
        
        var expectedResult = new TotalBalanceDataViewModel()
        {
            TotalBalance = 0,
            BalanceLastMonth = 0,
            DifferenceFromLastToCurrentMonthPercentage = 0
        };
        
        // Act
        var result = transactionRepository.getTotalBalanceData("userId");
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    // get total balance data end
    
    // get total period expense data start
    [Fact]
    public void getTotalPeriodExpensesDataSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
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
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
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
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalPeriodExpensesDataViewModel()
        {
            TotalAmountOfExpenses = 770,
            AmountOfExpensesLastMonth = 20,
            DifferenceBetweenThisAndLastMonth = 3650
        };
        
        // Act
        var result = transactionRepository.getTotalPeriodExpensesData(user.Id);
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public void getTotalPeriodExpensesDataSuccessLastMonthWasMoreThanThisMonthTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
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
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 600,
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalPeriodExpensesDataViewModel()
        {
            TotalAmountOfExpenses = 770,
            AmountOfExpensesLastMonth = 620,
            DifferenceBetweenThisAndLastMonth = (decimal)-75.81
        };
        
        // Act
        var result = transactionRepository.getTotalPeriodExpensesData(user.Id);
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public void getTotalPeriodExpensesDataSuccessLastMonthNoDataTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalPeriodExpensesDataViewModel()
        {
            TotalAmountOfExpenses = 0,
            AmountOfExpensesLastMonth = 0,
            DifferenceBetweenThisAndLastMonth = 0
        };
        
        // Act
        var result = transactionRepository.getTotalPeriodExpensesData("userId");
        
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    // get total period expense data end
    
    // Get Expense Total Amount For All Categories Last Month start
    [Fact]
    public void GetExpenseTotalAmountForAllCategoriesLastMonthSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);
        
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
                Category = categories[0],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 30,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow,
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 10,
                Category = categories[1],
                ApplicationUser = user,
            },
            new Transaction()
            {
                Title = "Test2",
                Description = "Test2",
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 600,
                Category = categories[2],
                ApplicationUser = user,
            }
        };

        var budgets = new List<Budget>
        {
            new Budget() { Amount = 110, Category = categories[0] },
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

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );
        
        // Act
        var result = transactionRepository.GetExpenseTotalAmountForAllCategoriesLastMonth(user.Id);
        
        // Assert
        result.Should().Be(620);
    }
    
    [Fact]
    public void GetExpenseTotalAmountForAllCategoriesLastMonthNoDataTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using var context = new ApplicationDbContext(options);

        var transactionRepository = new TransactionRepository(
            context,
            mockCategoryRepository.Object,
            mockBudgetRepository.Object
        );

        var expectedResult = new TotalPeriodExpensesDataViewModel()
        {
            TotalAmountOfExpenses = 0,
            AmountOfExpensesLastMonth = 0,
            DifferenceBetweenThisAndLastMonth = 0
        };
        
        // Act
        var result = transactionRepository.GetExpenseTotalAmountForAllCategoriesLastMonth("userId");
        
        // Assert
        result.Should().Be(0);
    }
    // Get Expense Total Amount For All Categories Last Month end
}