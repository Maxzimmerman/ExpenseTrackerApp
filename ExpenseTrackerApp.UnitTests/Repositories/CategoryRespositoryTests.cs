using System.Collections;
using Moq;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class CategoryRespositoryTests
{
    private Mock<ICategoryTypeRepsitory> mockCategoryTypeRepository = new Mock<ICategoryTypeRepsitory>();
    private Mock<ICategoryIconRepository> mockCategoryIconRepository = new Mock<ICategoryIconRepository>();
    private Mock<ICategoryColorRepository> mockCategoryColorRepository = new Mock<ICategoryColorRepository>();
    private Mock<Lazy<ITransactionRepository>> mockTransactionRepository = new Mock<Lazy<ITransactionRepository>>();
    
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    // addCategoryData Tests Start
    [Fact]
    public void addCategoryDataNoDataSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypesAsSelectListItems())
            .Returns(new List<SelectListItem>());
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconsAsSelectListItems())
            .Returns(new List<SelectListItem>());
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorsAsSelectListItems())
            .Returns(new List<SelectListItem>());

        var categoryRepo = new CategoryRepository
        (
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        var result = categoryRepo.addCategoryData("userid");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CategoryTypes.Count());
        Assert.Equal(0, result.CategoryIcons.Count());
        Assert.Equal(0, result.CategoryColors.Count());
        Assert.Equal(0, result.Expenses.Count());
        Assert.Equal(0, result.Incomes.Count());
    }
    // addCategoryData Test End
}