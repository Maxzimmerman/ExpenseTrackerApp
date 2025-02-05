using ExpenseTrackerApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class TransactionRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public void getMonthlyAverageForCertainCategorySuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            
        }
    }
}